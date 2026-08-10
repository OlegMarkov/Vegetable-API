using System.Collections.Generic;
using com.gexin.rp.sdk.dto;
using com.igetui.api.openservice;
using com.igetui.api.openservice.igetui;
using com.igetui.api.openservice.igetui.template;
using com.igetui.api.openservice.payload;
using com.igetui.api.openservice.igetui.template.notify;
using com.igetui.api.openservice.igetui.template.style;
using System.Threading.Tasks;
using GeTuiPushApiV2.ServerSDK.Core;
using System;
using GeTuiPushApiV2.ServerSDK.Storage;
using Newtonsoft.Json;

namespace Vegetable.Core.Services
{
    public class PushService : IPushService
    {

        private readonly GeTuiPushService _pushService;
        private readonly GeTuiPushOptions _options;
        private readonly GeTuiPushV2Api _api;
        private readonly IStorage _iStorage;

        public PushService(GeTuiPushService pushService, GeTuiPushOptions options, GeTuiPushV2Api api, IStorage iStorage)
        {
            _pushService = pushService;
            _options = options;
            _api = api;
            _iStorage = iStorage;
        }

        private async Task<string> GetTokenAsync(string appId, bool forceRefresh = false)
        {
            string token = _iStorage.GetToken(appId);
            if (string.IsNullOrEmpty(token) || forceRefresh)
            {
                _ = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000L) / 10000;
                token = (await _pushService.AuthAsync()).data.token;
            }

            return token;
        }

        public async Task<string> PushMessageToSingleAsync(string cid, string title, string content, string urlId = "", string platform = "ios")
        {
            ApiPushToSingleInDto apiInDto = null;

            if (string.IsNullOrEmpty(platform))
            {
                platform = "ios";
            }

            if (platform == "ios")
            {
                apiInDto = new ApiPushToSingleInDto
                {
                    request_id = Guid.NewGuid().ToString(),
                    audience = new audience_cidDto
                    {
                        cid = new string[] { cid }
                    },                   
                    push_message = new push_messageDto()
                    {
                        transmission = JsonConvert.SerializeObject(new
                        {
                            title = title,
                            body = content,
                            url = urlId
                        })                       
                    },
                    push_channel = new push_channelDto()
                    {                        
                        ios = new iosDto()
                        {
                            aps = new apsDto()
                            {
                                alert = new alertDto()
                                {
                                    title = title,
                                    body = content
                                },
                                sound = "default"
                            },
                            auto_badge = "+1"
                        }
                    }
                };

                if (!string.IsNullOrEmpty(urlId))
                {
                    apiInDto.push_channel.ios.payload = JsonConvert.SerializeObject(new
                    {
                        url = urlId
                    });
                }
            }
            else
            {
                apiInDto = new ApiPushToSingleInDto
                {
                    request_id = Guid.NewGuid().ToString(),
                    audience = new audience_cidDto
                    {
                        cid = new string[] { cid }
                    },
                    settings = new settingsDto
                    {
                        ttl = 600
                    },
                    push_message = new push_messageDto()
                    {
                        notification = new notificationDto
                        {
                            title = title,
                            body = content,
                            click_type = "startapp",
                            logo = "push144_circle.png",
                            badge_add_num = 1,
                            channel_level = 3,
                            notify_id = new Random().Next(0, 2147483647)
                        }
                    },
                    push_channel = new push_channelDto()
                    {
                        android = new androidDto()
                        {
                            ups = new upsDto()
                            {
                                notification = new notificationDto()
                                {
                                    title = title,
                                    body = content,
                                    click_type = "startapp",
                                    logo = "push144_circle.png",
                                    badge_add_num = 1,
                                    channel_level = 3,
                                    notify_id = new Random().Next(0, 2147483647)
                                }
                            }
                        }                        
                    }
                };

                if (!string.IsNullOrEmpty(urlId))
                {
                    apiInDto.push_message.notification.click_type = "payload";
                    apiInDto.push_message.notification.payload = JsonConvert.SerializeObject(new
                    {
                        url = urlId
                    });
                    apiInDto.push_channel.android.ups.notification.click_type = "payload";
                    apiInDto.push_channel.android.ups.notification.payload = JsonConvert.SerializeObject(new
                    {
                        url = urlId
                    });                    
                }
            }

            ApiPushToSingleInDto apiPushToSingleInDto = apiInDto;
            apiPushToSingleInDto.token = await GetTokenAsync(_options.AppID);
            apiInDto.appId = _options.AppID;
            ApiResultOutDto<ApiPushToSingleOutDto> result = await _api.PushToSingleAsync(apiInDto);
            if (result.code.Equals(10001))
            {
                ApiPushToSingleInDto apiPushToSingleInDto2 = apiInDto;
                apiPushToSingleInDto2.token = await GetTokenAsync(_options.AppID, forceRefresh: true);
                result = await _api.PushToSingleAsync(apiInDto);
            }

            return result.msg;
        }




        // These were literals holding a second, drifting copy of the values in
        // appsettings.json. Credentials do not belong in source at all, and two
        // copies of one credential is how a rotation half-happens. They now come
        // from the injected options, which come from configuration.
        private readonly string HOST = "http://sdk.open.api.igexin.com/apiex.htm";
        private string APPID => _options.AppID;
        private string APPKEY => _options.AppKey;
        private string MASTERSECRET => _options.MasterSecret;

        //public async Task<string> PushMessageToSingleAsync(string cid, string title, string content, string urlId = "")
        //{
        //    return await Task<string>.Run(() =>
        //    {
        //        IGtPush push = new IGtPush(HOST, APPKEY, MASTERSECRET);
        //        var template = CreateLinkTemplate(title, content, urlId);
        //        //var template = CreateNotificationTemplate(title, content);
        //        // Single push message model
        //        SingleMessage message = new SingleMessage();
        //        // When the user is not online, whether to store offline, optional
        //        message.IsOffline = true;
        //        // Offline effective time, in milliseconds, optional
        //        message.OfflineExpireTime = 1000 * 3600 * 12;
        //        message.Data = template;

        //        // Judge whether the client is pushed in a wifi environment, 2 is 4G/3G/2G, 1 is in a WIFI environment, 0 is an unlimited environment
        //        // message.PushNetWorkType = 1; 
        //        com.igetui.api.openservice.igetui.Target target = new com.igetui.api.openservice.igetui.Target();
        //        target.appId = APPID;
        //        target.clientId = cid;
        //        try
        //        {
        //            var pushResult = push.pushMessageToSingle(message, target);
        //            return pushResult;
        //        }
        //        catch (RequestException e)
        //        {
        //            var requestId = e.RequestId;
        //            // Retransmission after failed transmission
        //            var pushResult = push.pushMessageToSingle(message, target, requestId);
        //            return pushResult;
        //        }
        //    });
        //}

        public void PushMessageToApp()
        {
            // Push the main class (method 1, cannot coexist with method 2)
            IGtPush push = new IGtPush(HOST, APPKEY, MASTERSECRET);

            // Push the main category (method 2, cannot coexist with method 1) 
            // This method can push the message after obtaining the server address list to determine the fastest domain name, and check the fastest domain name every 10 minutes
            //IGtPush push = new IGtPush("",APPKEY,MASTERSECRET);

            AppMessage message = new AppMessage();

            // Set the push speed of the group push interface, the unit is per second, only valid for pushMessageToApp (for the specified application group push interface)
            message.Speed = 100;

            NotificationTemplate template = CreateNotificationTemplate(null, null);

            // When the user is not online, whether to store offline, optional
            message.IsOffline = false;
            // Offline effective time, in milliseconds, optional 
            message.OfflineExpireTime = 1000 * 3600 * 12;
            message.Data = template;
            // Judge whether the client is pushing in the wifi environment, 
            // 1 means in the WIFI environment, 0 means no restriction on the network environment  
            //message.PushNetWorkType = 0;        
            List<string> appIdList = new List<string>();
            appIdList.Add(APPID);

            //Notification recipient’s mobile operating system type
            List<string> phoneTypeList = new List<string>();
            //phoneTypeList.Add("ANDROID");
            //phoneTypeList.Add("IOS");

            // Notify recipient's province
            List<string> provinceList = new List<string>();

            List<string> tagList = new List<string>();

            message.AppIdList = appIdList;
            message.PhoneTypeList = phoneTypeList;
            message.ProvinceList = provinceList;
            message.TagList = tagList;

            var pushResult = push.pushMessageToApp(message);
        }

        // Web template content
        private LinkTemplate CreateLinkTemplate(string title, string content, string urlId = "")
        {
            LinkTemplate template = new LinkTemplate();
            template.AppId = APPID;
            template.AppKey = APPKEY;
            // Notification column title
            template.Title = title;
            // Notification bar content 
            template.Text = content;
            // Notification bar local pictures
            template.Logo = "push144_circle.png";
            // Notification bar displays the network icon, if it cannot be read, the local default icon is displayed, which can be empty
            template.LogoURL = "";

            // Open link address   
            template.Url = $"vegetable://com.vegetable.mob/{urlId}";
            // Whether to ring the received message, true: ring false: not ring  
            template.IsRing = true;
            // Whether the received message vibrates, true: vibrates false: no vibration  
            template.IsVibrate = true;
            // Whether the received message can be cleared, true: clearable false: unclearable
            template.IsClearable = true;

            //ios
            APNPayload apnpayload = new APNPayload();
            apnpayload.Badge = 1;
            apnpayload.Sound = "com.gexin.ios.silence";
            apnpayload.addCustomMsg("payload", "payloadMessage");
            apnpayload.ContentAvailable = 1;
            apnpayload.Category = "ACTIONABLE";
            apnpayload.VoicePlayType = 2;
            apnpayload.VoicePlayMessage = "New busy carrot notification";

            DictionaryAlertMsg alertMsg = new DictionaryAlertMsg();
            alertMsg.Body = content;
            alertMsg.ActionLocKey = "actionLockey";
            alertMsg.LocKey = "lockey";
            List<string> locargs = new List<string>();
            locargs.Add("locArgs");
            alertMsg.LocArgs = locargs;
            alertMsg.LaunchImage = "launchImage";
            // Supported by IOS8.2 and above;
            alertMsg.Title = title;
            List<string> TitleLocArg = new List<string>();
            TitleLocArg.Add("TitleLocArg");
            alertMsg.TitleLocArgs = TitleLocArg;
            alertMsg.TitleLocKey = "TitleLocKey";
            apnpayload.AlertMsg = alertMsg;

            template.setAPNInfo(apnpayload);

            return template;
        }

        private TransmissionTemplate CreateTransmissionTemplate(string title, string content)
        {
            TransmissionTemplate template = new TransmissionTemplate();
            template.AppId = APPID;
            template.AppKey = APPKEY;
            // Application startup type, 1: Force application startup 2: Wait for application startup
            template.TransmissionType = 1;
            // Transparent content  
            template.TransmissionContent = "Content";
            Notify notify = new Notify();
            notify.Title = title;
            notify.Content = content;
            // notify.Intent = "intent:#Intent;mm;end";
            //notify.Payload='payloadtest';
            //notify.Type = NotifyInfo.Types.Type._payload;
            notify.Intent = $"intent:#Intent;action=android.intent.action.oppopush;launchFlags=0x14000000;component=您的安卓包名/io.dcloud.PandoraEntry;S.UP-OL-SU=true;S.title={title};S.content={content};S.payload=test;end";
            notify.Type = NotifyInfo.Types.Type._intent;
            template.set3rdNotifyInfo(notify);
            APNPayload apnpayload = new APNPayload();
            apnpayload.Badge = 1;
            apnpayload.Sound = "com.gexin.ios.silence";
            apnpayload.addCustomMsg("payload", "payloadMessage");
            apnpayload.ContentAvailable = 1;
            apnpayload.Category = "ACTIONABLE";
            apnpayload.VoicePlayType = 2;
            apnpayload.VoicePlayMessage = "New veg notification";
            DictionaryAlertMsg alertMsg = new DictionaryAlertMsg();
            alertMsg.Body = "body";
            alertMsg.ActionLocKey = "actionLockey";
            alertMsg.LocKey = "lockey";
            List<string> locargs = new List<string>();
            locargs.Add("locArgs");
            alertMsg.LocArgs = locargs;
            alertMsg.LaunchImage = "launchImage";
            // Supported by IOS8.2 and above;
            alertMsg.Title = "Title";
            List<string> TitleLocArg = new List<string>();
            TitleLocArg.Add("TitleLocArg");
            alertMsg.TitleLocArgs = TitleLocArg;
            alertMsg.TitleLocKey = "TitleLocKey";
            apnpayload.AlertMsg = alertMsg;
            //Style0 style = new Style0();
            //style.Title = "123123";
            //style.Text = "1231";
            //style.Logo = "";
            //style.IsClearable = true;
            //style.IsRing = true;
            //style.IsVibrate = true;
            // Set the notification timing display time. 
            // The difference between the end time and the start time must be more than 6 minutes. 
            // After the message is pushed, the client will display the message within the specified time difference (error 6 minutes)
            //String begin = "2015-03-06 14:36:10";
            //String end = "2015-03-06 14:46:20";
            //template.setDuration(begin, end);
            //VoIPPayload voIPPayload = new VoIPPayload();
            //voIPPayload.voIPPayload = "getui";
            template.setAPNInfo(apnpayload);
            return template;
        }

        // Notification transparent transmission template action content
        private NotificationTemplate CreateNotificationTemplate(string title, string content)
        {
            NotificationTemplate template = new NotificationTemplate();
            template.AppId = APPID;
            template.AppKey = APPKEY;
            // Notification column title
            template.Title = title;
            // Notification bar content     
            template.Text = content;
            // Notification bar local pictures
            template.Logo = "push144_circle.png";
            // Notification bar shows the network icon
            template.LogoURL = "";
            // Application startup type, 1: Force application startup 2: Wait for application startup
            template.TransmissionType = 1;
            // Transparent content
            template.TransmissionContent = "Transparent content";
            template.IsRing = true;
            template.IsVibrate = true;
            template.IsClearable = true;
            // Set the notification timing display time. 
            // The difference between the end time and the start time must be more than 6 minutes. 
            // After the message is pushed, the client will display the message within the specified time difference (error 6 minutes)
            //string begin = "2015-03-06 14:36:10";
            //string end = "2015-03-06 14:46:20";
            //template.setDuration(begin, end);

            APNPayload apnpayload = new APNPayload();
            apnpayload.Badge = 1;
            apnpayload.Sound = "com.gexin.ios.silence";
            apnpayload.addCustomMsg("payload", "payloadMessage");
            apnpayload.ContentAvailable = 1;
            apnpayload.Category = "ACTIONABLE";
            apnpayload.VoicePlayType = 2;
            apnpayload.VoicePlayMessage = "New busy carrot notification";

            DictionaryAlertMsg alertMsg = new DictionaryAlertMsg();
            alertMsg.Body = content;
            alertMsg.ActionLocKey = "actionLockey";
            alertMsg.LocKey = "lockey";
            List<string> locargs = new List<string>();
            locargs.Add("locArgs");
            alertMsg.LocArgs = locargs;
            alertMsg.LaunchImage = "launchImage";
            // Supported by IOS8.2 and above;
            alertMsg.Title = title;
            List<string> TitleLocArg = new List<string>();
            TitleLocArg.Add("TitleLocArg");
            alertMsg.TitleLocArgs = TitleLocArg;
            alertMsg.TitleLocKey = "TitleLocKey";
            apnpayload.AlertMsg = alertMsg;

            template.setAPNInfo(apnpayload);

            return template;
        }

        private StartActivityTemplate CreateActivityNotificationTemplate(string title, string content)
        {
            StartActivityTemplate template = new StartActivityTemplate();
            template.AppId = APPID;
            template.AppKey = APPKEY;
            // template.TransmissionType = 1;
            var style = new Style0();
            // Notification column title
            style.Title = title;
            // Notification bar content     
            style.Text = content;
            // Notification bar local pictures
            style.Logo = "push144_circle.png";
            // Notification bar shows the network icon
            // style.LogoURL = "";
            // Application startup type, 1: Force application startup 2: Wait for application startup
            //style.TransmissionType = 1;
            // Transparent content
            //style.TransmissionContent = "Transparent content";
            style.IsRing = true;
            style.IsVibrate = true;
            style.IsClearable = true;

            template.setStyle(style);

            var intent = "intent:#Intent;component=com.vegetable.mob/reservation/edit;end";
            template.setIntent(intent);

            //var notify = new Notify();           
            //notify.Title = "title";
            //notify.Content = "url"; 
            //template.set3rdNotifyInfo(notify);
            //var intent = "intent://Intent;component=com.yourpackage/.NewsActivity;end";
            // template.setPushInfo()
            // Set the notification timing display time. 
            // The difference between the end time and the start time must be more than 6 minutes. 
            // After the message is pushed, the client will display the message within the specified time difference (error 6 minutes)
            //string begin = "2015-03-06 14:36:10";
            //string end = "2015-03-06 14:46:20";
            //template.setDuration(begin, end);

            APNPayload apnpayload = new APNPayload();
            apnpayload.Badge = 1;
            apnpayload.Sound = "com.gexin.ios.silence";
            apnpayload.addCustomMsg("payload", "payloadMessage");
            apnpayload.ContentAvailable = 1;
            apnpayload.Category = "ACTIONABLE";
            apnpayload.VoicePlayType = 2;
            apnpayload.VoicePlayMessage = "New busy carrot notification";

            DictionaryAlertMsg alertMsg = new DictionaryAlertMsg();
            alertMsg.Body = content;
            alertMsg.ActionLocKey = "actionLockey";
            alertMsg.LocKey = "lockey";
            List<string> locargs = new List<string>();
            locargs.Add("locArgs");
            alertMsg.LocArgs = locargs;
            alertMsg.LaunchImage = "launchImage";
            // Supported by IOS8.2 and above;
            alertMsg.Title = title;
            List<string> TitleLocArg = new List<string>();
            TitleLocArg.Add("TitleLocArg");
            alertMsg.TitleLocArgs = TitleLocArg;
            alertMsg.TitleLocKey = "TitleLocKey";
            apnpayload.AlertMsg = alertMsg;

            template.setAPNInfo(apnpayload);

            return template;
        }
    }
}
