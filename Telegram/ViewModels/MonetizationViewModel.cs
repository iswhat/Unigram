//
// Copyright Fela Ameghino 2015-2024
//
// Distributed under the GNU General Public License v3.0. (See accompanying
// file LICENSE or copy at https://www.gnu.org/licenses/gpl-3.0.txt)
//
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Common;
using Telegram.Navigation;
using Telegram.Navigation.Services;
using Telegram.Services;
using Telegram.Td.Api;
using Telegram.ViewModels.Chats;
using Telegram.ViewModels.Profile;
using Telegram.Views;
using Telegram.Views.Chats;
using Windows.UI.Xaml.Navigation;

namespace Telegram.ViewModels
{
    public class MonetizationViewModel : MultiViewModelBase, IHandle
    {
        protected readonly ChatStatisticsViewModel _statisticsViewModel;
        protected readonly ChatBoostsViewModel _boostsViewModel;
        protected readonly ChatMonetizationViewModel _monetizationViewModel;

        public MonetizationViewModel(IClientService clientService, ISettingsService settingsService, IEventAggregator aggregator)
            : base(clientService, settingsService, aggregator)
        {
            _statisticsViewModel = TypeResolver.Current.Resolve<ChatStatisticsViewModel>(clientService.SessionId);
            _boostsViewModel = TypeResolver.Current.Resolve<ChatBoostsViewModel>(clientService.SessionId);
            _monetizationViewModel = TypeResolver.Current.Resolve<ChatMonetizationViewModel>(clientService.SessionId);

            Children.Add(_statisticsViewModel);
            Children.Add(_boostsViewModel);
            Children.Add(_monetizationViewModel);

            Items = new ObservableCollection<ProfileTabItem>
            {
                new ProfileTabItem(Strings.Statistics, typeof(ChatStatisticsPage)),
                new ProfileTabItem(Strings.Boosts, typeof(ChatBoostsPage)),
            };

            if (Constants.DEBUG)
            {
                Items.Add(new ProfileTabItem(Strings.Monetization, typeof(ChatMonetizationPage)));
            }
        }

        public ObservableCollection<ProfileTabItem> Items { get; }

        public ChatStatisticsViewModel Statistics => _statisticsViewModel;
        public ChatBoostsViewModel Boosts => _boostsViewModel;
        public ChatMonetizationViewModel Monetization => _monetizationViewModel;

        protected override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, NavigationState state)
        {
            var chatId = (long)parameter;

            if (state.TryGet("selectedIndex", out int selectedIndex))
            {
                SelectedIndex = selectedIndex;
            }

            Chat = ClientService.GetChat(chatId);

            SelectedItem ??= Items.FirstOrDefault();
            RaisePropertyChanged(nameof(SharedCount));
        }

        private int[] _sharedCount = new int[] { 0, 0, 0, 0, 0, 0 };
        public int[] SharedCount
        {
            get => _sharedCount;
            set => Set(ref _sharedCount, value);
        }

        private ProfileTabItem _selectedItem;
        public ProfileTabItem SelectedItem
        {
            get => _selectedItem;
            set => Set(ref _selectedItem, value);
        }

        protected Chat _chat;
        public Chat Chat
        {
            get => _chat;
            set => Set(ref _chat, value);
        }

        private int _selectedIndex;
        public int SelectedIndex
        {
            get => _selectedIndex;
            set => Set(ref _selectedIndex, value);
        }

        private double _headerHeight;
        public double HeaderHeight
        {
            get => _headerHeight;
            set
            {
                if (Set(ref _headerHeight, value))
                {
                    //Statistics.HeaderHeight = value;
                    //Boosts.HeaderHeight = value;
                }
            }
        }
    }
}