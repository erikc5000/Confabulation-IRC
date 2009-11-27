using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Confabulation.Chat;

namespace Confabulation
{
	public class ChannelUserItem : DependencyObject
	{
		public ChannelUserItem(IrcChannelUser channelUser)
		{
			//this.channelUser = channelUser;

			Nickname = channelUser.Nickname;
			IsOperator = channelUser.IsOperator;
			IsHalfOperator = channelUser.IsHalfOperator;
			HasVoice = channelUser.HasVoice;
		}

		public static readonly DependencyProperty NicknameProperty =
			DependencyProperty.Register("Nickname", typeof(string), typeof(ChannelUserItem));

		public static readonly DependencyProperty IsOperatorProperty =
			DependencyProperty.Register("IsOperator", typeof(bool), typeof(ChannelUserItem));

		public static readonly DependencyProperty IsHalfOperatorProperty =
			DependencyProperty.Register("IsHalfOperator", typeof(bool), typeof(ChannelUserItem));

		public static readonly DependencyProperty HasVoiceProperty =
			DependencyProperty.Register("HasVoice", typeof(bool), typeof(ChannelUserItem));

		public string Nickname
		{
			get { return (string)GetValue(NicknameProperty); }
			set { SetValue(NicknameProperty, value); }
		}

		public bool IsOperator
		{
			get { return (bool)GetValue(IsOperatorProperty); }
			set { SetValue(IsOperatorProperty, value); }
		}

		public bool IsHalfOperator
		{
			get { return (bool)GetValue(IsHalfOperatorProperty); }
			set { SetValue(IsHalfOperatorProperty, value); }
		}

		public bool HasVoice
		{
			get { return (bool)GetValue(HasVoiceProperty); }
			set { SetValue(HasVoiceProperty, value); }
		}

		//private IrcChannelUser channelUser;
	}
}
