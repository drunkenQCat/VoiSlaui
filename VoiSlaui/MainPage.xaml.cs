using VoiSlauiLib.Models;
using VoiSlauiLib.Helper;
using andEnv = Android.OS.Environment;
using VoiSlauiLib.Utilities;

namespace VoiSlaui;
public partial class MainPage : ContentPage
{
    List<SlateLogItem> logItemList = new();
    FileLoadingHelper fhelper = FileLoadingHelper.Instance;


    public MainPage()
    {
        InitializeComponent();
        CopyRecordsBtn.IsEnabled = false;
    }

    private async void OnSlatePickerClicked(object sender, EventArgs e)
    {
#if ANDROID
        var pickResult = await FilePicker.Default.PickAsync();

        if (pickResult is not null)
        {
            Console.WriteLine("Selected folder: " + pickResult.FullPath);
            fhelper.GetLogs(pickResult.FullPath);
            logItemList = fhelper.LogList;
            await DisplayAlert("场记读取成功", $"一共{logItemList.Count}条场记已加载", "确定");
        }
        else
        {
            await DisplayAlert("读取失败", $"错误信息: {pickResult.FileName}", "确定");
        }
        CopyRecordsBtn.IsEnabled = true;
        // Todo:Loading...
#endif
    }

    private async void OnCopyClicked(object sender, EventArgs e)
    {
#if ANDROID
        var desPath = andEnv.GetExternalStoragePublicDirectory(andEnv.DirectoryMovies);
        SafService safService = await pickRecordFolder();
        await copyRecords(safService);
        await readBwfInfo();

        await writeMetadata();

        static async Task<SafService> pickRecordFolder()
        {
            //选取录音文件夹
            SafService safService = new();
            Task pickRecordFolder = new TaskFactory().StartNew(() => safService.ShowUriBrowser());
            await pickRecordFolder;
            return safService;
        }

        async Task copyRecords(SafService safService)
        {
            progressIndicator.Text = "正在复制文件到Movies文件夹";
            Task copyAllRecords = new TaskFactory().StartNew(
                () => safService.CopyAllFromExternalStorage(desPath.AbsolutePath)
                );
            progressBar.ProgressTo(0.75, 100000, Easing.Linear);
            await copyAllRecords;
            progressBar.ProgressTo(0.85, 1, Easing.Linear);

        }

        async Task readBwfInfo()
        {
            progressIndicator.Text = "正在读取录音文件元数据";
            //var destinyPath = Path.Combine(desPath.AbsolutePath, SafService.FolderName);
            var destinyPath = Path.Combine(desPath.AbsolutePath, "09-21-23");
            int matchedCount = fhelper.GetBwf(destinyPath);
            await progressBar.ProgressTo(1, 100, Easing.CubicIn);
            progressIndicator.Text = "复制完成";
            await DisplayAlert("录音已复制到Movies", $"{matchedCount}条录音已匹配，即将写入元数据", "点击继续");
        }

        async Task writeMetadata()
        {
            // 写入元数据
            SubscribeProgress();
            progressIndicator.Text = "正在对录音备份写入元数据";
            Task task = new TaskFactory().StartNew(() => fhelper.WriteMetaData());
            await task;
            progressIndicator.Text = "元数据已写入完成，请安全移除SD卡";
            await DisplayAlert("写入完成", "元数据已写入，请安全移除SD卡", "确定");
        }
#endif
    }

    private void SubscribeProgress()
    {
        ProgressBlock.Instance.ProgressHandler += (_, progressCount) =>
        {
            progressBar.ProgressTo(progressCount, 0, Easing.CubicIn);
        };
    }

}
