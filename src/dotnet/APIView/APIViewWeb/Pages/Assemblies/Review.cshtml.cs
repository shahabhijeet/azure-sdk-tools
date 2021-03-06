﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIView;
using APIViewWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace APIViewWeb.Pages.Assemblies
{
    public class ReviewModel : PageModel
    {
        private readonly BlobAssemblyRepository assemblyRepository;
        private readonly BlobCommentRepository commentRepository;

        public ReviewModel(BlobAssemblyRepository assemblyRepository, BlobCommentRepository commentRepository)
        {
            this.assemblyRepository = assemblyRepository;
            this.commentRepository = commentRepository;
        }

        public string Id { get; set; }
        public LineAPIV[] AssemblyModel { get; set; }
        [BindProperty]
        public CommentModel Comment { get; set; }
        public Dictionary<string, List<CommentModel>> Comments { get; set; }
        public string Username { get; set; }

        public async Task<ActionResult> OnPostDeleteAsync(string id, string commentId, string elementId)
        {
            await commentRepository.DeleteCommentAsync(commentId);
            var commentArray = await commentRepository.FetchCommentsAsync(id);
            List<CommentModel> comments = commentArray.Where(comment => comment.ElementId == elementId).ToList();

            CommentThreadModel partialModel = new CommentThreadModel()
            {
                AssemblyId = id,
                Comments = comments,
                LineId = Comment.ElementId
            };

            return new PartialViewResult
            {
                ViewName = "_CommentThreadPartial",
                ViewData = new ViewDataDictionary<CommentThreadModel>(ViewData, partialModel)
            };
        }

        public async Task OnGetAsync(string id)
        {
            Id = id;
            var assemblyModel = await assemblyRepository.ReadAssemblyContentAsync(id);
            var renderer = new HTMLRendererAPIV();
            AssemblyModel = renderer.Render(assemblyModel.Assembly).ToArray();
            var comments = await commentRepository.FetchCommentsAsync(id);

            Comments = new Dictionary<string, List<CommentModel>>();
            foreach (var comment in comments)
            {
                if (!Comments.TryGetValue(comment.ElementId, out List<CommentModel> list))
                    Comments[comment.ElementId] = new List<CommentModel>() { comment };
                else
                    Comments[comment.ElementId].Add(comment);
            }

            Username = User.GetGitHubLogin();
        }

        public async Task<ActionResult> OnPostAsync(string id)
        {
            await commentRepository.UploadCommentAsync(Comment, id);
            var commentArray = await commentRepository.FetchCommentsAsync(id);
            List<CommentModel> comments = commentArray.Where(comment => comment.ElementId == Comment.ElementId).ToList();

            CommentThreadModel partialModel = new CommentThreadModel()
            {
                AssemblyId = id,
                Comments = comments,
                LineId = Comment.ElementId
            };

            return new PartialViewResult
            {
                ViewName = "_CommentThreadPartial",
                ViewData = new ViewDataDictionary<CommentThreadModel>(ViewData, partialModel)
            };
        }
    }
}
