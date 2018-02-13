﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoweredSoft.DynamicLinq.Dal.Pocos
{
    public class Comment
    {
        public long Id { get; set; }
        public long? ParentCommentId { get; set; }
        public long PostId { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string CommentText { get; set; }

        public Comment ParentComment { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public Post Post { get; set; }
    }
}