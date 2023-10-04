using VoiSlauiLib.Models;
using VoiSlauiLib.Helper;
using CommunityToolkit.Maui.Views;
using VoiSlaui.Views;
using andEnv = Android.OS.Environment;
using VoiSlauiLib.Utilities;

namespace VoiSlaui;
public partial class MainPage : ContentPage
{
    int count = 0;
    List<SlateLogItem> logItemList = new();
    FileLoadingHelper fhelper = FileLoadingHelper.Instance;
    WriteProgressPopView writeProg = new ();

    public bool IsWritingEnabled { get; private set; }

    public MainPage()
    {
        InitializeComponent();
    }

    private void OnCounterClicked(object sender, EventArgs e)
    {
        count++;

        if (count == 1)
            CounterBtn.Text = $"Clicked {count} time";
        else
            CounterBtn.Text = $"点了 {count} 次";

        SemanticScreenReader.Announce(CounterBtn.Text);
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
        }
        else
        {
            await DisplayAlert("读取失败", $"错误信息: {pickResult.FileName}", "确定");
        }
        IsWritingEnabled = false;
        // Todo:Loading...
#endif
    }

    private async void OnCopyClicked(object sender, EventArgs e)
    {
#if ANDROID
        // 选取录音文件夹
        SafService safService = new();
        safService.ShowUriBrowser();

        // 复制录音
        var copyProg = new CopyProgressPopView();
        this.ShowPopup(copyProg);
        var desPath = andEnv.GetExternalStoragePublicDirectory(andEnv.DirectoryMovies);
        Task copyAllRecords = new TaskFactory().StartNew(
            () => safService.CopyAllFromExternalStorage(desPath.AbsolutePath)
            );
        await copyAllRecords;
        await copyProg.prog.ProgressTo(0.75, 200, Easing.CubicIn);
        
        var destinyPath = Path.Combine(desPath.AbsolutePath, SafService.FolderName);
        int matchedCount = fhelper.GetBwf(destinyPath);
        await copyProg.prog.ProgressTo(1, 100, Easing.CubicIn);
        copyProg.Close();
        await DisplayAlert("录音已复制到Movies", $"{matchedCount}条录音已匹配，即将写入元数据", "点击继续");

        // 写入元数据
        this.ShowPopup(writeProg);
        Task task = new TaskFactory().StartNew(() => fhelper.WriteMetaData());
        await task;
        writeProg.Close();
        IsWritingEnabled = true;
        await DisplayAlert("写入完成", "元数据已写入，请安全移除SD卡", "确定");
#endif
    }

    private void SubscribeProgress()
    {
        ProgressBlock.Instance.ProgressHandler += (_, progressCount) =>
        {
            writeProg.prog.ProgressTo(progressCount, 1, Easing.CubicIn);
        };
    }

}
