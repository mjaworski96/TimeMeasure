using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.Widget;
using TimeMeasure.ViewModel;

namespace TimeMeasure.Droid.Widget
{
    [BroadcastReceiver(Label = "Time Measure Widget")]
    [IntentFilter(new string[] { "android.appwidget.action.APPWIDGET_UPDATE" })]
    [MetaData("android.appwidget.provider", Resource = "@xml/appwidgetprovider")]
    public class AppWidget : AppWidgetProvider
    {
        private static string ButtonClick = "ButtonClickTag";
        private BindingContext bindingContext = new BindingContext(null);

        public override void OnUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
        {
            var me = new ComponentName(context, Java.Lang.Class.FromType(typeof(AppWidget)).Name);
            appWidgetManager.UpdateAppWidget(me, BuildRemoteViews(context, appWidgetIds));
        }

        private RemoteViews BuildRemoteViews(Context context, int[] appWidgetIds)
        {
            var widgetView = new RemoteViews(context.PackageName, Resource.Layout.Widget);

            SetTextViewText(widgetView);
            RegisterClicks(context, appWidgetIds, widgetView);

            return widgetView;
        }

        private void SetTextViewText(RemoteViews widgetView)
        {
            UpdateTime(widgetView);
        }
        private void UpdateTime(RemoteViews widgetView)
        {
            widgetView.SetTextViewText(Resource.Id.widgetDay, "Day: " + bindingContext.DayTotalTime);
            widgetView.SetTextViewText(Resource.Id.widgetWeek, "Week: " + bindingContext.WeekTotalTime);
            widgetView.SetTextViewText(Resource.Id.widgetMonth, "Month: " + bindingContext.MonthTotalTime);
            widgetView.SetTextViewText(Resource.Id.widgetTotal, "Total: " + bindingContext.TotalTime);
        }
        private void RegisterClicks(Context context, int[] appWidgetIds, RemoteViews widgetView)
        {
            Intent intent = new Intent(context, typeof(AppWidget));
            intent.SetAction(AppWidgetManager.ActionAppwidgetUpdate);
            intent.PutExtra(AppWidgetManager.ExtraAppwidgetIds, appWidgetIds);

            PendingIntent piBackground = PendingIntent.GetBroadcast(context, 0, intent, PendingIntentFlags.UpdateCurrent);
            widgetView.SetOnClickPendingIntent(Resource.Id.widgetBackground, piBackground);

            widgetView.SetOnClickPendingIntent(Resource.Id.widgetButton, GetPendingSelfIntent(context, ButtonClick));
        }

        private PendingIntent GetPendingSelfIntent(Context context, string action)
        {
            Intent intent = new Intent(context, typeof(AppWidget));
            intent.SetAction(action);
            return PendingIntent.GetBroadcast(context, 0, intent, 0);
        }

        public override void OnReceive(Context context, Intent intent)
        {
            base.OnReceive(context, intent);

            if (ButtonClick.Equals(intent.Action))
            {
                this.bindingContext.MainButtonCommand.Execute(null);
            }
        }
    }
}