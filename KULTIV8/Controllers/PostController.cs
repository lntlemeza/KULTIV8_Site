using KULTIV8.Data;
using KULTIV8.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Configuration;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using NuGet.Versioning;

namespace KULTIV8.Controllers
{
    public class PostController : Controller
    {
        private readonly ApplicationDBContext _db;
        private readonly IWebHostEnvironment _env;

        public PostController(ApplicationDBContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        /*This Controller has 5 sections
            1. Post CRUD Operations
            2. Drafts CRUD Operations
            3. Comments CRUD Operations
            4. Searching
            5. Blog Posts
         */

        /********************/

        /* 1. Post CRUD operations */

        /// <summary>
        /// Index View. Gets all the posts that have been published
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            IEnumerable<Post> posts = _db.Posts
                .Include(post => post.User)
                .Where(post => post.Status == "published")
                .OrderByDescending(post => post.DatePublished);

           


            return View(posts);
        }

        /// <summary>
        /// Gets a post and returns the Detail View*/
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public IActionResult PostDetails(int? id, Dictionary<string, string> routeData)
        {
            
            if (routeData != null && routeData.ContainsKey("isBlog"))
                ViewBag.isBlog = routeData["isBlog"];

            if (id == null)
            {
                return NotFound();
            }
            else
            {

                var post = _db.Posts.Include(p => p.User).Where(p => p.Id == id).FirstOrDefault();
                return View(post);

            }

           

        }

        /// <summary>
        /// Handles loading the page for the new Post and also for loading the post to be updated
        /// </summary>
        /// <param name="act"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public IActionResult PostEditor(string act, string value) {

            if (act == "Update") {
                var post = _db.Posts.Find(Int32.Parse(value));
                if(post == null)
                {
                    return NotFound();
                }
                return View(post);
            }

            return View();
        }

        /// <summary>
        /// Handles the updating of a post*/
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult UpdatePost(Post post) {
            var filepath = "";
            foreach (IFormFile photo in Request.Form.Files)
            {
                string guid = Guid.NewGuid().ToString();
                string serverMapPath = Path.Combine(_env.WebRootPath, "img/post/covers", guid + "-" + photo.FileName);
                using (var stream = new FileStream(serverMapPath, FileMode.Create))
                {
                    photo.CopyTo(stream);
                }

                filepath = Url.Content("~/img/post/covers/" + guid + "-" + photo.FileName);
                post.PhotoUrl = filepath;
            }

           
            post.LastUpdate = DateTime.Now;

            try
            {
                _db.Posts.Update(post);
                _db.SaveChanges();
                TempData["Message"] = "Success: Post succesfully updated";
                TempData["MessageType"] = "success";

            }
            catch(Exception)
            {
                TempData["Message"] = "Warning: Something went wrong. Make sure all fields have been filled";
                TempData["MessageType"] = "danger";
                return View(post);
            }

            return RedirectToAction("Index");



        }

        /// <summary>
        /// Submits the post to the database
        /// Submits drafts into the database
        /// </summary>
        /// <param name="_post"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult NewPost(Post _post, string action)
        {

            
            _post.Status = (action == "draft") ? "draft" : "published"; 

            //Checks the Id of a draft, to determine whether it already exists or not. 
            //If it exists, it should be updated
            if(_post.Id != 0)
            {
                return UpdatePost(_post);
            }

            //Handle the Image that's coming through with the form
            var filepath = "";
            foreach (IFormFile photo in Request.Form.Files)
            {
                string guid = Guid.NewGuid().ToString();
                string serverMapPath = Path.Combine(_env.WebRootPath, "img/post/covers", guid + "-" + photo.FileName);
                using (var stream = new FileStream(serverMapPath, FileMode.Create))
                {
                    photo.CopyTo(stream);
                }
                
                filepath = Url.Content("~/img/post/covers/" + guid + "-" + photo.FileName);
            }

            _post.PhotoUrl = filepath;
            _post.DatePublished = DateTime.Now;
            _post.LastUpdate = DateTime.Now;    
            

            try
            {
                _db.Posts.Add(_post);
                _db.SaveChanges();
                TempData["Message"] = (action == "draft") ? "Success: Draft has been saved" : "Success: Post has been published";
                TempData["MessageType"] = "success";
            }
            catch (Exception exp)
            {
                TempData["Message"] = "Warning: " + exp.ToString();
                TempData["MessageType"] = "danger";
                return RedirectToAction(nameof(PostEditor));
            }
            
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Deletes a post 
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(int? Id)
        {
            var post = _db.Posts.Find(Id);
            if(post == null){
                return NotFound();
            }

            try
            {
                _db.Posts.Remove(post);
                _db.SaveChanges();
                TempData["Message"] = "Success: Post has been deleted";
                TempData["MessageType"] = "success";
            }
            catch(Exception exp){
                TempData["Message"] = "Warning: " + exp.ToString();
                TempData["MessageType"] = "danger";
                return BadRequest(exp.Message);
            }
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Handles the image to be uploaded with the  content of the post
        /// The image is saved in the root folder and the method returns the url of the image in Json format
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadImage(List<IFormFile> file)
        {
            var filepath = "";
            string guid = Guid.NewGuid().ToString();
            foreach (IFormFile photo in Request.Form.Files)
            {
                string serverMapPath = Path.Combine(_env.WebRootPath, "img/post/intext", guid + "-" + photo.FileName);
                using (var stream = new FileStream(serverMapPath, FileMode.Create))
                {
                    photo.CopyTo(stream);
                }
                filepath = Url.Content("~/img/post/intext/" + guid + "-" + photo.FileName);

            }


            return Json(new {url = filepath});
        }


        /********************/

        /* 2. Drafts */
        [HttpGet]
        public IActionResult GetDrafts()
        {
            IEnumerable<Post> drafts = _db.Posts
                .Include(draft => draft.User)
                .Where(draft => draft.Status == "draft")
                .OrderByDescending(draft => draft.DatePublished); ;
            return View("Index", drafts);
        }

        /********************/
        
        /* 3. Comments */

        /* 5. Blog Post */
        public IActionResult Blog()
        {
            IEnumerable<Post> posts = _db.Posts.Include(post => post.User)
                .Where(post => post.Status == "published")
                .OrderByDescending(post => post.DatePublished);

            return View(posts);
            
        }

    }
}
