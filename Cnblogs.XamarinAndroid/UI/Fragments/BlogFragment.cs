using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Support.V4.View;
using Fragment = Android.Support.V4.App.Fragment;
using Android.Support.Design.Widget;

namespace Cnblogs.XamarinAndroid
{
    public class BlogFragment : Fragment, TabLayout.IOnTabSelectedListener
    {
        private ViewPager _viewPagerHome;
        private TabLayout _tabHome;
        private BlogFragmentTabsAdapter adapter;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            HasOptionsMenu = true;
            //Android.Support.V7.Widget.Toolbar toolbar = Activity.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            //toolbar.Title = Resources.GetString(Resource.String.CnblogsTitle);
            // Create your fragment here
        }
        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            Activity.MenuInflater.Inflate(Resource.Menu.search, menu);
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            base.OnCreateView(inflater, container, savedInstanceState);
            return   inflater.Inflate(Resource.Layout.fragment_home, container, false);
        }
        public override void OnViewCreated(View view,Bundle savedInstanceState)
        {
            base.OnViewCreated(view,savedInstanceState);
            HasOptionsMenu=true;
            _viewPagerHome = view.FindViewById<ViewPager>(Resource.Id.viewPager_home);
            _tabHome = view.FindViewById<TabLayout>(Resource.Id.tab_home);

            //List<string> list = new List<string>() { "����","����"};

            adapter=new BlogFragmentTabsAdapter(this.ChildFragmentManager,Resources.GetStringArray(Resource.Array.HomeTabs));

            _viewPagerHome.Adapter = adapter;
            _tabHome.TabMode = TabLayout.ModeFixed;
            _tabHome.TabGravity = TabLayout.GravityFill;
            _tabHome.SetupWithViewPager(_viewPagerHome);
            _tabHome.SetOnTabSelectedListener(this);

        }

        public void OnTabReselected(TabLayout.Tab tab)
        {
            
        }

        public void OnTabSelected(TabLayout.Tab tab)
        {
            _viewPagerHome.CurrentItem = tab.Position;
            
        }

        public void OnTabUnselected(TabLayout.Tab tab)
        {
            
        }
    }
}