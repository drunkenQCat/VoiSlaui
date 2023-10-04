using CommunityToolkit.Maui.Views;

namespace VoiSlaui.Views;

public partial class CopyProgressPopView : Popup
{
	

    public ProgressBar prog;
	public CopyProgressPopView()
	{
		prog = copyProgressBar;
		InitializeComponent();
	}
}