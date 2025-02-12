using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using Android.Util;
using Com.Nostra13.Universalimageloader.Core;
using Android.Content.Res;
using Cnblogs.HttpClient;

namespace Cnblogs.XamarinAndroid
{
    public interface ILongClick: View.IOnLongClickListener
    {
       
    }
    public class BaseHolder : RecyclerView.ViewHolder,View.IOnLongClickListener
    {
        private SparseArray<View> views;
        private Context context;
        private BaseHolder(Context context, View itemView, Action<int,string> click, Action<string, int> longClick) : base(itemView)
        {
            this.context = context;
            views = new SparseArray<View>(8);
            ItemView.Click += (s, e) => click(base.Position,itemView.Tag.ToString());
            
            ItemView.LongClick += (s, e) => longClick(itemView.Tag.ToString(), Position);
            // ItemView.SetOnLongClickListener(this);
        }
        private BaseHolder(Context context, View itemView, Action<string, int> longClick) : base(itemView)
        {
            this.context = context;
            views = new SparseArray<View>(8);
            ItemView.LongClick += (s, e) => longClick(itemView.Tag.ToString(), Position);
            //ItemView.LongClick+= (s, e) => listener(base.Position, itemView.Tag.ToString());
            // ItemView.SetOnLongClickListener(this);
        }
        public BaseHolder(View view) : base(view)
        {
        }
        public static BaseHolder GetRecyclerHolder(Context context, View itemView, Action<int,string> click,Action<string,int> longClick)
        {
            return new BaseHolder(context, itemView, click,longClick);
        }
        public SparseArray<View> GetViews()
        {
            return this.views;
        }
        public T GetView<T>(int viewId) where T : View
        {
            View view = views.Get(viewId);
            if (view == null)
            {
                view = ItemView.FindViewById(viewId);
                views.Put(viewId, view);
            }
            return (T)view;
        }
        public BaseHolder SetText(int viewId, string text)
        {
            TextView tv = GetView<TextView>(viewId);
            tv.Text = text;
            return this;
        }
        public BaseHolder SetImageLoader(int viewId, DisplayImageOptions options,string  url)
        {
            ImageView iv = GetView<ImageView>(viewId);
            //string url = iv.Tag.ToString();
            System.Diagnostics.Debug.Write("imgaeUrl",url);
            if (string.IsNullOrEmpty(url) || url.Length < 4||!url.Substring(url.Length - 4, 4).Contains(".png"))
               ImageLoader.Instance.DisplayImage("",iv,options);
            else
                ImageLoader.Instance.DisplayImage(url, iv, options);
            return this;
        }
        public BaseHolder SetTag(int viewId, string id)
        {
            LinearLayout tv = GetView<LinearLayout>(viewId);
            tv.Tag = id;
            return this;
        }
        public BaseHolder SetTagUrl(int viewId,string  url)
        {
            ImageView iv = GetView<ImageView>(viewId);
            iv.Tag = url;
            return this;
        }

        public bool OnLongClick(View v)
        {
            throw new NotImplementedException();
        }
    }

    public class FooterViewHolder : RecyclerView.ViewHolder
    {
        public FooterViewHolder(View view) : base(view)
        {
        }
    }
    public class EmptyViewHolder : RecyclerView.ViewHolder
    {
        public EmptyViewHolder(View view) : base(view)
        {
        }
    }



    public class BaseRecyclerViewAdapter<T> : RecyclerView.Adapter
    {
            private Context context; //上下文
            private List<T> list;//数据源
            private LayoutInflater inflater;//布局器
            private int itemLayoutId; // 布局ID
            private RecyclerView _recyclerView;
            public delegate void Convert(BaseHolder holder, int position);
            public event Convert OnConvertView;
            public Action<int,string> ItemClick; //子view
            public Action<string,int> ItemLongClick; //子view
            private const int VIEW_ITEM = 0;
            private const int VIEW_FOOTER = 1;
            private const int VIEW_NULL = 2;
            private View footerView, emptyView;
            public delegate void InsertData();//添加更多数据的委托
            private InsertData _InsertDataEvent; //加载更多的事件
            public override int ItemCount
            {
                get
                {
                  if (list != null)
                  {
                    if (list.Count == 0)
                    {
                        return list.Count + 1;
                        return EmptyViewCount();
                    }
                    return list.Count +1;
                  }
                return 0;
                }
            }
            private int FooterViewCount()
        {
            if (footerView==null)
            {
                return 0;
            }
            return 1;
        }
            private int EmptyViewCount()
        {
            if (emptyView == null)
            {
                return 0;
            }
            return 1;
        }
            public override int GetItemViewType(int position)
        {
            if (ItemCount == 0)
            {
                return VIEW_NULL;
            }
            if (position  == ItemCount-1)
            {
                if (position == Constact.PageSize - 1)
                {
                    return VIEW_ITEM;
                }
                if (ItemCount < Constact.PageSize)
                {
                    if (ItemCount == 0)
                        return VIEW_NULL;
                    else
                        return VIEW_NULL;
                        //return VIEW_ITEM;
                }
                _InsertDataEvent();
                return VIEW_FOOTER;
            }
            return VIEW_ITEM;
        }

            public void SetNewData(List<T> list)
            {
              this.list = list == null ? new List<T>() : list;
              NotifyDataSetChanged();
            }
        //在RecyclerView提供数据的时候调用
        public override void OnAttachedToRecyclerView(RecyclerView recyclerView)
        {
                base.OnAttachedToRecyclerView(recyclerView);
                this._recyclerView = recyclerView;
         }
         public override void OnDetachedFromRecyclerView(RecyclerView recyclerView)
         {
                base.OnDetachedFromRecyclerView(recyclerView);
                this._recyclerView = null;
         }

        public BaseRecyclerViewAdapter(Context context, List<T> list, int itemLayoutId, InsertData _InsertDataEvent)
        {
            this.context = context;
            this.list = list;
            this.itemLayoutId = itemLayoutId;
            this._InsertDataEvent = _InsertDataEvent;
            inflater = LayoutInflater.From(context);
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            if (viewType == VIEW_FOOTER)
            {
                footerView = inflater.Inflate(Resource.Layout.item_recyclerView_footer_loading,parent,false);
                return new  FooterViewHolder(footerView);
            }

            if (viewType == VIEW_NULL)
            {
                emptyView = inflater.Inflate(Resource.Layout.item_recyclerView_footer_empty, parent, false);
                return new EmptyViewHolder(emptyView);
            }
            View view = inflater.Inflate(itemLayoutId, parent, false);
            return BaseHolder.GetRecyclerHolder(context, view, ItemClick,ItemLongClick);
        }
         public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
         {
              if(holder is BaseHolder)
              {
                //myHolder.ItemView.Tag = position;
                //myHolder.
                BaseHolder myHolder = holder as BaseHolder;
                OnConvertView(myHolder, position);
              }
          }
     }
}