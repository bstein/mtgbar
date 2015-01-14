using System.Windows;
using System.Windows.Controls;
using MtGBar.Infrastructure.DataNinjitsu.Models.TweetWords;

namespace MtGBar.Infrastructure.UIHelpers.DataTemplateSelectors
{
    public class TweetWordDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate HashtagTemplate { get; set; }
        public DataTemplate MentionTemplate { get; set; }
        public DataTemplate TweetWordTemplate { get; set; }
        public DataTemplate UrlTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item != null) {
                if (item.GetType() == typeof(Hashtag)) {
                    return HashtagTemplate;
                }
                else if (item.GetType() == typeof(Mention)) {
                    return MentionTemplate;
                }
                else if (item.GetType() == typeof(Url)) {
                    return UrlTemplate;
                }
                else {
                    return TweetWordTemplate;
                }
            }

            return null;
        }
    }
}
