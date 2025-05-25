using UnityEngine;
using YNL.Utilities.Addons;

namespace YNL.Checkotel
{
	public class ViewManager : MonoBehaviour
	{
		public ViewType CurrentViewType;
        public bool IsAbleToMovePage = true;

		public SerializableDictionary<ViewType, ViewPageUI> Pages = new();

        public void Awake()
        {
            Marker.OnViewPageSwitched += OnViewPageSwitched;
        }

        private void OnDestroy()
        {
            Marker.OnViewPageSwitched -= OnViewPageSwitched;
        }

        private void OnViewPageSwitched(ViewType type, bool hidePreviousPage, bool needRefresh)
        {
            if (!IsAbleToMovePage) return;

            if (hidePreviousPage)
            {
                Pages[CurrentViewType].DisplayView(false, needRefresh);
            }

            CurrentViewType = type;

            Pages[CurrentViewType].DisplayView(true, needRefresh);
        }
    }

    public enum ViewType : byte
    {
        SigningViewSignUpPage, 
        SigningViewSignInPage,
        
        MainViewHomePage,
        MainViewFavoritePage,
        MainViewBookingPage,
        MainViewAccountPage,
        
        SearchViewMainPage, 
        SearchViewResultPage,
        
        InformationViewMainPage,
        InformationViewReviewPage,
        InformationViewFacilitiesPage,
        InformationViewRoomPage,
        InformationViewPaymentPage
    }
}