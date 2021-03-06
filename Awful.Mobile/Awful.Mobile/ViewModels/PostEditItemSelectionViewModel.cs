﻿// <copyright file="PostEditItemSelectionViewModel.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Awful.Core.Entities.PostIcons;
using Awful.Database.Context;
using Awful.Database.Entities;
using Awful.Mobile.Controls;
using Awful.UI.Actions;
using Awful.UI.ViewModels;
using Forms9Patch;
using Xamarin.Forms;

namespace Awful.Mobile.ViewModels
{
    /// <summary>
    /// Post Edit Items Seleciton View Model.
    /// </summary>
    public class PostEditItemSelectionViewModel : MobileAwfulViewModel
    {
        private Editor editor;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostEditItemSelectionViewModel"/> class.
        /// </summary>
        public PostEditItemSelectionViewModel()
        {
        }

        /// <summary>
        /// Gets the list of Edit Post Items.
        /// </summary>
        public List<EditPostItem> EditPostItems { get
            {
            return new List<EditPostItem>()
            {
                new EditPostItem()
                {
                    Type = EditPostItemType.Emotes,
                    Glyph = "",
                    Title = "Emotes",
                },
                new EditPostItem()
                {
                    Type = EditPostItemType.InsertImgur,
                    Glyph = "",
                    Title = "Insert Imgur",
                },
                new EditPostItem()
                {
                    Type = EditPostItemType.InsertVideo,
                    Glyph = "",
                    Title = "Insert Video",
                },
                new EditPostItem()
                {
                    Type = EditPostItemType.InsertUrl,
                    Glyph = "",
                    Title = "Insert URL",
                },
                new EditPostItem()
                {
                    Type = EditPostItemType.QuoteBlock,
                    Glyph = "",
                    Title = "Quote Block",
                },
                new EditPostItem()
                {
                    Type = EditPostItemType.List,
                    Glyph = "",
                    Title = "List",
                },
                new EditPostItem()
                {
                    Type = EditPostItemType.CodeBlock,
                    Glyph = "",
                    Title = "Code Block",
                },
                new EditPostItem()
                {
                    Type = EditPostItemType.PreserveSpace,
                    Glyph = "",
                    Title = "Preserve Space",
                },
                new EditPostItem()
                {
                    Type = EditPostItemType.Bold,
                    Glyph = "",
                    Title = "Bold",
                },
                new EditPostItem()
                {
                    Type = EditPostItemType.Italics,
                    Glyph = "",
                    Title = "Italics",
                },
                new EditPostItem()
                {
                    Type = EditPostItemType.Underline,
                    Glyph = "",
                    Title = "Underline",
                },
                new EditPostItem()
                {
                    Type = EditPostItemType.Strikeout,
                    Glyph = "",
                    Title = "Strikeout",
                },
                new EditPostItem()
                {
                    Type = EditPostItemType.SpoilerText,
                    Glyph = "",
                    Title = "Spoiler Text",
                },
                new EditPostItem()
                {
                    Type = EditPostItemType.Superscript,
                    Glyph = "",
                    Title = "Superscript",
                },
                new EditPostItem()
                {
                    Type = EditPostItemType.Subscript,
                    Glyph = "",
                    Title = "Subscript",
                },
                new EditPostItem()
                {
                    Type = EditPostItemType.FixedWidth,
                    Glyph = "",
                    Title = "Fixed Width",
                },
            };
            }
        }

        /// <summary>
        /// Loads editor into VM.
        /// </summary>
        /// <param name="editor">SA Post Editor.</param>
        public void LoadEditor(Editor editor)
        {
            this.editor = editor;
        }
    }
}
