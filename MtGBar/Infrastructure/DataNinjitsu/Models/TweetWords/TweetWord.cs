namespace MtGBar.Infrastructure.DataNinjitsu.Models.TweetWords
{
    public class TweetWord
    {
        public string Text { get; set; }

        public TweetWord(string text)
        {
            this.Text = text;
        }
    }
}