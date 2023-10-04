using CommunityToolkit.Maui.Views;

namespace VoiSlaui.Views;

public partial class WriteProgressPopView : Popup
{
	

    public ProgressBar prog;
	public WriteProgressPopView()
	{
		prog = writeProgressBar;
		InitializeComponent();
	}
}