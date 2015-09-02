﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
using Newtonsoft.Json;
using OfflineMediaV3.Business.Enums.Models;
using OfflineMediaV3.Business.Framework.Repositories.Interfaces;
using OfflineMediaV3.Business.Models.Configuration;
using OfflineMediaV3.Business.Models.NewsModel;
using OfflineMediaV3.Business.Sources.Stern.Models;
using OfflineMediaV3.Common.Framework.Logs;
using OfflineMediaV3.Common.Helpers;

namespace OfflineMediaV3.Business.Sources.Stern
{
    public class SternHelper : IMediaSourceHelper
    {
        public async Task<List<ArticleModel>> EvaluateFeed(string feed, SourceConfigurationModel scm, FeedConfigurationModel fcm)
        {
            var articlelist = new List<ArticleModel>();
            if (feed != null)
            {
                try
                {
                    var f = JsonConvert.DeserializeObject<SternFeed>(feed);
                    foreach (var entry in f.entries)
                    {
                        if (entry.type == "teaserlist")
                        {
                            articlelist.AddRange(entry.entries.Select(rawarticles => FeedToArticleModel(rawarticles, scm)).Where(article => article != null));
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Instance.Log(LogLevel.Error, this, "NzzHelper.EvaluateFeed failed", ex);
                }
            }
            return articlelist;
        }

        public ArticleModel FeedToArticleModel(Entry2 nfa, SourceConfigurationModel scm)
        {
            if (nfa == null) return null;

            try
            {
                if (nfa.articleType == "standard-article")
                {
                    var a = new ArticleModel
                    {
                        PublicationTime = DateTime.Parse(nfa.timestamp),
                        Title = nfa.kicker,
                        SubTitle = nfa.headline,
                        Teaser = nfa.teaser,
                    };

                    if (nfa.images != null && nfa.images.Count > 3)
                        a.LeadImage = new ImageModel()
                        {
                            Url = new Uri(nfa.images[3].src)
                        };

                    a.LogicUri = new Uri(scm.LogicBaseUrl + nfa.contentId + ".json");
                    a.PublicUri = new Uri(scm.PublicBaseUrl + nfa.contentId);

                    return a;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "SternHelper.FeedToArticleModel failed", ex);
            }
            return null;
        }

        public bool NeedsToEvaluateArticle()
        {
            return true;
        }

        public async Task<Tuple<bool, ArticleModel>> EvaluateArticle(string article, ArticleModel am)
        {
            if (article == null) return new Tuple<bool, ArticleModel>(false, am);

            try
            {
                article = article.Replace("[[]]", "[]");
                var a = JsonConvert.DeserializeObject<SternArticle>(article);
                return await ArticleToArticleModel(a, am);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "NzzHelper.EvaluateArticle failed", ex);
            }
            return new Tuple<bool, ArticleModel>(false, am);
        }

        public bool WriteProperties(ref ArticleModel original, ArticleModel evaluatedArticle)
        {
            original.Content = evaluatedArticle.Content;
            original.Author = evaluatedArticle.Author;
            original.Themes = evaluatedArticle.Themes;
            return true;
        }

        public List<string> GetKeywords(ArticleModel articleModel)
        {
            var part1 = TextHelper.Instance.GetImportantWords(articleModel.Title);
            var part2 = TextHelper.Instance.GetImportantWords(articleModel.SubTitle);

            return TextHelper.Instance.FusionLists(part1, part2);
        }

        public async Task<Tuple<bool, ArticleModel>> ArticleToArticleModel(SternArticle na, ArticleModel am)
        {
            if (na != null)
            {
                try
                {
                    am.Content = new List<ContentModel>()
                    {
                        new ContentModel()
                        {
                            ContentType = ContentType.Html,
                            Html = GetHtml(na.content)
                        }
                    };

                    if (na.head?.credits != null)
                    {
                        am.Author = na.head.credits.author;
                    }
                   
                    var repo = SimpleIoc.Default.GetInstance<IThemeRepository>();
                    am.Themes = new List<ThemeModel> {await repo.GetThemeModelFor(am.FeedConfiguration.Name)};

                    return new Tuple<bool, ArticleModel>(true, am);
                }
                catch (Exception ex)
                {
                    LogHelper.Instance.Log(LogLevel.Error, this, "SternHelper.ArticleToArticleModel deserialization failed", ex);
                }
            }
            return new Tuple<bool, ArticleModel>(false, am);
        }

        private string GetHtml(List<Content> children)
        {
            var res = "";
            if (children != null)
                foreach (var child in children)
                {
                    if (child.type == "p")
                        res += "<p>" + GetHtml(child.children) + "</p>";
                    else if (child.type == "strong")
                        res += "<strong>" + GetHtml(child.children) + "</strong>";
                    else if (child.type == "text")
                        res += child.content;
                    else if (child.type == "a")
                        res += "<a href=\"" + child.href + "\">" + GetHtml(child.children) + "</a>";
                }


            return res;
        }

        private string GetHtml(List<Content2> children)
        {
            var res = "";
            if (children != null)
                foreach (var child in children)
                {
                    if (child.type == "p")
                        res += "<p>" + GetHtml(child.children) + "</p>";
                    else if (child.type == "strong")
                        res += "<strong>" + GetHtml(child.children) + "</strong>";
                    else if (child.type == "text")
                        res += child.content;
                    else if (child.type == "a")
                        res += "<a href=\"" + child.href + "\">" + GetHtml(child.children) + "</a>";
                }


            return res;
        }
    }
}
