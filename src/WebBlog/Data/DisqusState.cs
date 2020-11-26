using System;

namespace WebBlog.Data
{
    public class DisqusState
    {
        private bool _DisplayDisqus = false;

        public event EventHandler StateChanged;

        public bool getDisplayDisqus()
        {
            return _DisplayDisqus;
        }

        public void SetDisplayDisqus(bool param)
        {
            _DisplayDisqus = param;
            StateHasChanged();
        }

        private void StateHasChanged()
        {
            StateChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
