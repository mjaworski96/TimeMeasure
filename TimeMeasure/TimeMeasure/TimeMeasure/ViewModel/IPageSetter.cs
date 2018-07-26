namespace TimeMeasure.ViewModel
{
    public interface IPageSetter
    {
        void SetMainPage();
        void SetEditorPage(EditorBindingContext bindingContext);
    }
}
