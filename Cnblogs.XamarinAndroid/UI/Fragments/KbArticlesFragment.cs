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
using Cnblogs.ApiModel;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Cnblogs.XamarinAndroid.UI;
using Cnblogs.HttpClient;
using System.Threading.Tasks;
using Cnblogs.XamarinAndroid.UI.Widgets;

namespace Cnblogs.XamarinAndroid
{
    public class KbArticlesFragment : Fragment, SwipeRefreshLayout.IOnRefreshListener
    {
        private RecyclerView _recyclerView;
        private SwipeRefreshLayout _swipeRefreshLayout;
        private BaseRecyclerViewAdapter<KbArticles> adapter;
        private List<KbArticles> kbArticlesList = new List<KbArticles>();
        private int pageIndex=1;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            HasOptionsMenu=true;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
                base.OnCreateView(inflater, container, savedInstanceState);
               return inflater.Inflate(Resource.Layout.fragment_kbArticles, container, false);
        }

        public override async void OnViewCreated(View view,Bundle  savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            _swipeRefreshLayout = view.FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
            _swipeRefreshLayout.SetColorSchemeResources(Resource.Color.primary);
            _swipeRefreshLayout.SetOnRefreshListener(this);
            _swipeRefreshLayout.Post(() =>
            {
                _swipeRefreshLayout.Refreshing = true;
            });
            _swipeRefreshLayout.PostDelayed(() =>
            {
                System.Diagnostics.Debug.Write("PostDelayed方法已经完成");
                _swipeRefreshLayout.Refreshing = false;
            },3000);
            _recyclerView = view.FindViewById<RecyclerView>(Resource.Id.recyclerView);
            _recyclerView.SetLayoutManager(new Android.Support.V7.Widget.LinearLayoutManager(this.Activity));
            kbArticlesList =await listKbArticleLocal();
            if (kbArticlesList.Count > 0)
            {
                initRecycler();
            }
            else
            {
                kbArticlesList = await listKbArticlesServer();
                if (kbArticlesList.Count>0)
                {
                    initRecycler();
                }
            }
        }

        private async Task<List<KbArticles>> listKbArticlesServer()
        {
            var result = await KbArticlesService.ListKbArticle(AccessTokenUtil.GetToken(this.Activity), pageIndex);
            if (result.Success)
            {
                _swipeRefreshLayout.Refreshing = false;
                try
                {
                   
                    await SQLiteUtil.UpdateKbArticlesList(result.Data);
                    return result.Data;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Write(ex.ToString());
                    return null;
                }
            }
            return null;
        }
        private async Task<List<KbArticles>> listKbArticleLocal()
        {
            kbArticlesList = await SQLiteUtil.SelectKbArticleList(Constact.PageSize);
            return kbArticlesList;
        }

        async void initRecycler()
        {
            adapter = new BaseRecyclerViewAdapter<KbArticles>(this.Activity, kbArticlesList, Resource.Layout.item_recyclerview_kbarticles, LoadMore);
            _recyclerView.SetAdapter(adapter);
            adapter.ItemClick += (position, tag) =>
            {
                    System.Diagnostics.Debug.Write(position, tag);
                   // AlertUtil.ToastShort(this.Activity, tag);
                    DetailKbArticlesActivity.Enter(Activity, int.Parse(tag));
            };
            adapter.ItemLongClick += (tag, position) =>
            {
                //AlertUtil.ToastShort(this.Activity, tag);
            };
            adapter.OnConvertView += (holder, position) =>
            {
                holder.SetText(Resource.Id.tv_dateAdded, kbArticlesList[position].DateAdded.ToCommonString());
                holder.SetText(Resource.Id.tv_viewCount, kbArticlesList[position].ViewCount.ToString());
                holder.SetText(Resource.Id.tv_summary, kbArticlesList[position].Summary);
                holder.SetText(Resource.Id.tv_diggCount, kbArticlesList[position].Diggcount.ToString());
                holder.SetText(Resource.Id.tv_title, kbArticlesList[position].Title);
                holder.SetText(Resource.Id.tv_author,kbArticlesList[position].Author);
                holder.GetView<CardView>(Resource.Id.ly_item).Tag = kbArticlesList[position].Id.ToString();
            };
        }

        private async void LoadMore()
        {
            pageIndex++;
            var tempList = await listKbArticlesServer();
            kbArticlesList.AddRange(tempList);
            if (tempList.Count == 0)
            {
                return;
            }
            else if (kbArticlesList != null)
            {
                adapter.SetNewData(kbArticlesList);
                System.Diagnostics.Debug.Write("页数:" + pageIndex + "数据总条数：" + kbArticlesList.Count);
            }
        }

        public async void OnRefresh()
        {
            if (pageIndex > 1)
                pageIndex = 1;
            var tempList = await listKbArticlesServer();
            if (tempList != null)
            {
                kbArticlesList = tempList;
                _swipeRefreshLayout.Refreshing = false;
                adapter.SetNewData(tempList);
            }
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            //Activity.MenuInflater.Inflate(Resource.Menu.setting, menu);
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            return base.OnOptionsItemSelected(item);
        }
    }
}