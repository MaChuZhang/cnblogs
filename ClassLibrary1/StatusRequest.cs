﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using Newtonsoft.Json;
using Cnblogs.ApiModel;
using Cnblogs.HttpClient;

namespace Cnblogs.HttpClient
{
    public class StatusRequest
    {
        public static async Task<ApiResult<List<StatusModel>>> ListStatuses(Token token,int statusType,int pageIndex,int  pageSize)
        {
            try
            {
                string _statusType = "all";
                switch (statusType)
                {
                    case (int)StatusType.all:
                        _statusType = "all";
                        break;
                    case (int)StatusType.following:
                        _statusType = "following";
                        break;
                    case (int)StatusType.my:
                        _statusType = "following";
                        break;

                    case (int)StatusType.recentcomment:
                        _statusType = "recentcomment";
                        break;
                    case (int)StatusType.mention:
                        _statusType = "mention";
                        break;

                    case (int)StatusType.comment:
                        _statusType = "comment";
                        break;

                    default:
                        _statusType = "all";
                        break;
                }
                string url = string.Format(Constact.Statuses,_statusType,pageIndex,pageSize);
                var result=await HttpBase.GetAsync(url,null,token);
                if (result.IsSuccess)
                {
                    var list = JsonConvert.DeserializeObject<List<StatusModel>>(result.Message);
                    //successAction(list);
                    return  ApiResult.Ok(list);
                }
                else
                {
                    return ApiResult<List<StatusModel>>.Error(result.Message);
                   //errorAction(result.Message);
                }
            }
            catch (Exception ex)
            {
                //errorAction(ex.StackTrace.ToString());
                return ApiResult<List<StatusModel>>.Error(ex.Message);
            }
        }

        public static async Task<ApiResult<string>> GetArticleDetail(Token token,int id)
        {
            try
            {
                string url = string.Format(Constact.ArticleBody,id);
                var result = await HttpBase.GetAsync(url, null, token);
                if (result.IsSuccess)
                {
                    var articleDetail = result.Message;
                    //successAction(list);
                    return ApiResult.Ok(articleDetail);
                }
                else
                {
                    return ApiResult<string>.Error(result.Message);
                    //errorAction(result.Message);
                }
            }
            catch (Exception ex)
            {
                //errorAction(ex.StackTrace.ToString());
                return ApiResult<string>.Error(ex.Message);
            }
        }
    }
}