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
using Cnblogs.XamarinAndroid.Adapter;

namespace Cnblogs.XamarinAndroid
{
    public class QuestionFragment : Fragment, TabLayout.IOnTabSelectedListener
    {
            private ViewPager _viewPager;
            private TabLayout _tab;
            private QuestionTabFragmentAdapter adapter;
       
            public override void OnCreate(Bundle savedInstanceState)
            {
                base.OnCreate(savedInstanceState);
            }
            public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
            {
                Activity.MenuInflater.Inflate(Resource.Menu.question, menu);
            }
            public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
            {
                // Use this to return your custom view for this Fragment
                // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
                base.OnCreateView(inflater, container, savedInstanceState);
                return inflater.Inflate(Resource.Layout.fragment_home, container, false);
            }
            public override void OnViewCreated(View view, Bundle savedInstanceState)
            {
                base.OnViewCreated(view, savedInstanceState);
                HasOptionsMenu = true;
                _viewPager = view.FindViewById<ViewPager>(Resource.Id.viewPager_home);
                _tab = view.FindViewById<TabLayout>(Resource.Id.tab_home);
                string[] questionTabs = Resources.GetStringArray(Resource.Array.QuestionTabs);
                adapter = new QuestionTabFragmentAdapter(this.ChildFragmentManager, questionTabs);

            _viewPager.Adapter = adapter;
            _viewPager.OffscreenPageLimit = questionTabs.Length;
            _tab.TabMode = TabLayout.ModeFixed;
            _tab.SetupWithViewPager(_viewPager);
                _tab.SetOnTabSelectedListener(this);

            }

            public void OnTabReselected(TabLayout.Tab tab)
            {

            }

            public void OnTabSelected(TabLayout.Tab tab)
            {
                _viewPager.CurrentItem = tab.Position;

            }

            public void OnTabUnselected(TabLayout.Tab tab)
            {

            }
        }
}