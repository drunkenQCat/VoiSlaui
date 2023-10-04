namespace VoiSlaui;
using Microsoft.Maui.Controls;

public partial class MainPage : ContentPage
{
	int count = 0;

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
    private async void OnCopyClicked(object sender, EventArgs e)
    {
        //var pickResult = await FolderPicker.Default.PickAsync(CancellationToken.None);

        //if (pickResult.IsSuccessful)
        //{
        //    Console.WriteLine("Selected folder: " + pickResult.Folder.Name);
        //}
        //var docsDirectory = Android.App.Application.Context.GetExternalFilesDir(Android.OS.Environment.DirectoryDocuments);
        //File.WriteAllText($"{docsDirectory.AbsoluteFile.Path}/atextfile.txt", "contents are here");
#if ANDROID
        //var docsDirectory = Android.App.Application.Context.GetExternalFilesDir(Android.OS.Environment.DirectoryDcim);
        //File.WriteAllText($"{docsDirectory.AbsoluteFile.Path}/atextfile.txt", pickResult.Folder.Path);
        //using FileStream outputStream = System.IO.File.OpenWrite(targetFile);
        //using StreamWriter streamWriter = new StreamWriter(outputStream);
        //await streamWriter.WriteAsync("ssss");
#endif
#if ANDROID
        SafService service = new();
        service.ShowUriBrowser();
        SafService safService = new();
        var desPath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryMovies);
        safService.CopyAllFromExternalStorage(desPath.AbsolutePath);

        await DisplayAlert("content", "File Copied", "OK");
#endif
    }


    //read file
    private async void OnRecordPickerClicked(object sender, EventArgs e)
    {
#if ANDROID
        var pickResult = await FilePicker.Default.PickAsync();

        if (pickResult is not null)
        {
            Console.WriteLine("Selected folder: " + pickResult.FullPath);
        }
        var docsDirectory = Android.App.Application.Context.GetExternalFilesDir(Android.OS.Environment.DirectoryDocuments);
        File.WriteAllText($"{docsDirectory.AbsoluteFile.Path}/atextfile.txt", "contents are here");
        await DisplayAlert("content", "File Copied", "OK");
#endif
    }
}

