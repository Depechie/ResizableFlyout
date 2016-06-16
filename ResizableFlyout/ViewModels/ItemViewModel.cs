using System;
using Windows.Globalization.DateTimeFormatting;
using ResizableFlyout.Models;

namespace ResizableFlyout.ViewModels
{
    public class ItemViewModel
    {
        private int _itemId;
        public int ItemId => _itemId;

        public string DateCreatedHourMinute
        {
            get
            {
                var formatter = new DateTimeFormatter("hour minute");
                return formatter.Format(DateCreated);
            }
        }

        public string Title { get; set; }
        public string Text { get; set; }
        public DateTime DateCreated { get; set; }

        public ItemViewModel()
        {
        }

        public static ItemViewModel FromItem(Item item)
        {
            var viewModel = new ItemViewModel
            {
                _itemId = item.Id,
                DateCreated = item.DateCreated,
                Title = item.Title,
                Text = item.Text
            };

            return viewModel;
        }
    }
}
