﻿namespace RavenFS.Studio.Infrastructure.Input
{
	public class InputModel : NotifyPropertyChangedBase
	{
	    bool allowCancel = true;

	    public bool AllowCancel
	    {
	        get { return allowCancel; }
	        set
	        {
	            allowCancel = value;
	            OnPropertyChanged();
	        }
	    }

		private string title;

		public string Title
		{
			get { return title; }
			set
			{
				title = value;
				OnPropertyChanged();
			}
		}

		private string message;

		public string Message
		{
			get { return message; }
			set
			{
				message = value;
				OnPropertyChanged();
			}
		}

		private string answer;

		public string Answer
		{
			get { return answer; }
			set
			{
				answer = value;
				OnPropertyChanged();
			}
		}
	}
}