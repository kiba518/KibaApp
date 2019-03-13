using Android.App;
using Android.Widget;
using Android.Content;
using System;
using System.Json;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Android.Util;
using Android.Views;
using System.Linq;

namespace KibaApp
{
    [Activity(Label = "KibaApp")]
    public class BaseActivity : Activity
    {
        public void ShowActivity<T>() where T : Activity
        {
            Intent intent = new Intent(this, typeof(T));
            StartActivity(intent);
        }
        public void OpenService<T>() where T : Service
        {
            Intent intent = new Intent(this, typeof(T));
            StartService(intent);
        }

        #region  各种提示信息
        public void ShowToast(string msg)
        {
            Toast.MakeText(this, msg, ToastLength.Short).Show();
        }
       
        private AlertDialog dialog;
        public AlertDialog InitDialog(string msg, Action<AlertDialog> comfirmCallback, Action<AlertDialog> cancelCallback)
        {
            AlertDialog cdialog;
            //构造器
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            //标题
            builder.SetTitle("提示");
            //图标
            //builder.SetIcon(android.R.drawable.btn_dialog);
            //内容
            builder.SetMessage(msg);
            //setPositiveButton(表示按钮的文本，表示单击按钮触发单击事件)
            builder.SetPositiveButton("OK", new EventHandler<DialogClickEventArgs>((s, e) =>
            {
                if (comfirmCallback != null)
                {
                    comfirmCallback(dialog);
                }
            }));
            builder.SetNegativeButton("Cancel", new EventHandler<DialogClickEventArgs>((s, e) =>
            {
                if (cancelCallback != null)
                {
                    cancelCallback(dialog);
                }
            }));
            //builder.SetNeutralButton("稍后提醒", new EventHandler<DialogClickEventArgs>((s, e) => { }));
            cdialog = builder.Create();//构建dialog对象

            return cdialog;
        }

        public void ShowAlert(string msg, Action<AlertDialog> comfirmCallback = null, Action<AlertDialog> cancelCallback = null)
        {
            if (comfirmCallback == null)
            {
                cancelCallback = (d) => { dialog.Dismiss(); };
            }
            if (cancelCallback == null)
            {
                cancelCallback = (d) => { dialog.Dismiss(); };
            }
            dialog = InitDialog(msg, comfirmCallback, cancelCallback);
            if (dialog != null && !dialog.IsShowing)
            {
                dialog.Show();
            }
        }

        public void NotifyMessage(string message, string title = "消息")
        { 
            NotificationManager manager = (NotificationManager)GetSystemService(Context.NotificationService);  // 在Android进行通知处理，首先需要重系统哪里获得通知管理器NotificationManager，它是一个系统Service。  
            PendingIntent pendingIntent = PendingIntent.GetActivity(this, 0,
                    new Intent(this, typeof(MainActivity)), 0);
            Notification notify1 = new Notification();
            notify1.Icon = Resource.Drawable.logo;
            notify1.TickerText = JaveString("您有新短消息，请注意查收！");
            notify1.When = DateTime.Now.ToFileTime();
            notify1.SetLatestEventInfo(this, title, message, pendingIntent);
            notify1.Number = 1;
            notify1.Flags |= NotificationFlags.AutoCancel; // FLAG_AUTO_CANCEL表明当通知被用户点击时，通知将被清除。  
                                                           // 通过通知管理器来发起通知。如果id不同，则每click，在statu那里增加一个提示  
            manager.Notify(1, notify1); 
        }
        public static Java.Lang.String JaveString(string str)
        {
            return new Java.Lang.String("您有新短消息，请注意查收！");
        }
        #endregion

        #region 寻找资源
        public T FindControl<T>(string name) where T : View
        {
            return FindViewById<T>(GetCode(name));
        }

        public T FindControl<T>(string name, Action clickCallBack) where T : View
        {
            View view = FindViewById<T>(GetCode(name));
            view.Click += (s, e) =>
            {
                clickCallBack();
            };
            return FindViewById<T>(GetCode(name));
        } 

        public int GetCode(string name)
        {
            var R = this.Resources;
            var code = (typeof(Resource.Id)).GetFields().FirstOrDefault(f => f.Name == name).GetValue(R);
            return (int)code;
        }
        #endregion

        #region 异步调用
        public void AsyncLoad(Action action)
        {
            IAsyncResult result = action.BeginInvoke((iar) =>
            {
            }, null);
        }
        public void AsyncLoad(Action action, Action callback)
        {
            IAsyncResult result = action.BeginInvoke((iar) =>
            {
                this.RunOnUiThread(callback);
            }, null);
        }

        public void AsyncLoad<T>(Action<T> action, T para, Action callback)
        {
            IAsyncResult result = action.BeginInvoke(para, (iar) =>
            {
                this.RunOnUiThread(callback);
            }, null);
        }
        public void RunOnUi(Action action)
        {
            ((BaseActivity)this).RunOnUiThread(() => { action(); });//回UI线程
        }
        #endregion

        #region 获取数据
        public void GetRest(string url, Action<JsonValue> callback)
        {
            Task.Run(() =>
            {
                try
                {
                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
                    request.ContentType = "application/json";
                    request.Method = "GET";
                    using (WebResponse response = request.GetResponse())
                    {
                        using (Stream stream = response.GetResponseStream())
                        {
                            JsonValue jsonDoc = JsonObject.Load(stream);
                            callback(jsonDoc);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Debug("BaseActivity", $"Exception at GetRest" + ex.Message);
                }
            });

        }
        #endregion

    }
}

