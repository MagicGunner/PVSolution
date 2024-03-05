using System.ComponentModel;

namespace CADToolBox.Shared.Models.UIModels.Implement;

public class MyBindingList<T> : BindingList<T> {
    private bool _eventsEnabled = true;

    public new event ListChangedEventHandler? ListChanged;

    protected override void OnListChanged(ListChangedEventArgs e) {
        if (!_eventsEnabled) return;
        base.OnListChanged(e);
        ListChanged?.Invoke(this, e);
    }

    public void DisableEvents() {
        _eventsEnabled = false;
    }

    public void EnableEvents() {
        _eventsEnabled = true;
    }
}