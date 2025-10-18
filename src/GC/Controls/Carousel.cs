using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Threading;
using GC.Core;

namespace GC.Controls;

public class Carousel : UserControl {
  #region Fields ---------------------------------------------------------------
  private readonly TransitioningContentControl _contentControl;
  #endregion

  #region Properties -----------------------------------------------------------
  public List<MenuDayPanel> Items { get; set; } = [];

  public int CurrentIndex { get; private set; }

  public EventHandler<int> OnIndexChanged { get; set; } = (_, _) => { };
  public EventHandler OnPullDown { get; set; } = (_, _) => { };
  #endregion

  #region Constructors ---------------------------------------------------------
  public Carousel() {
    Log.Debug("Constructor called");
    CurrentIndex = 0;
    _contentControl = new TransitioningContentControl {
      PageTransition = new CrossFade(TimeSpan.FromSeconds(0.5))
    };
    Content = _contentControl;
    UpdateContent();
  }
  #endregion

  #region Methods --------------------------------------------------------------
  private void Next() {
    if (Items.Count == 0) return;
    Log.Debug($"Next: CurrentIndex={CurrentIndex}");
    CurrentIndex = (CurrentIndex + 1) % Items.Count;
    UpdateContent();
  }

  private void Previous() {
    if (Items.Count == 0) return;
    Log.Debug($"Previous: CurrentIndex={CurrentIndex}");
    CurrentIndex = (CurrentIndex - 1 + Items.Count) % Items.Count;
    UpdateContent();
  }

  public void GoToIndex(int index) {
    Log.Debug($"GoToIndex: {index}");
    while (index != CurrentIndex) {
      if (index > CurrentIndex)
        Next();
      else
        Previous();
    }
    UpdateContent();
  }
  #endregion

  #region Implementation -------------------------------------------------------
  private void UpdateContent() {
    Log.Debug($"UpdateContent: CurrentIndex={CurrentIndex} of {Items.Count}");
    if (CurrentIndex >= 0 && CurrentIndex < Items.Count) {
      _contentControl.Content = Items[CurrentIndex];
    }
    OnIndexChanged.Invoke(this, CurrentIndex);
    _pointer = default;
  }
  #endregion

  #region Swipe Support --------------------------------------------------------
  private (Point Start, Point Last) _pointer = (new Point(0, 0), new Point(0, 0));
  private const double SwipeThreshold = 30.0; // pixels required to consider a swipe

  public void SetupSwipe() {
    // Pointer pressed: record start position for this pointer
    PointerPressed += (_, e) => {
      e.Pointer.Capture(this);
      Log.Debug($"PointerPressed pos=({e.GetPosition(this).X:F1},{e.GetPosition(this).Y:F1})");
      _pointer.Start = e.GetPosition(this);
      _pointer.Last = _pointer.Start;

      // Per-pointer cleanup: if platform never sends PointerReleased (e.g. capture lost by system),
      // remove tracking after a short timeout so the pointer doesn't stay stuck.
      Task.Run(async () => {
        await Task.Delay(1500);
        Log.Debug("Pointer timeout cleanup");
        Dispatcher.UIThread.Post(() => { _pointer = default; });
      });
    };

    PointerMoved += (_, e) => {
      var pos = e.GetPosition(this);
      Log.Debug($"PointerMoved pos=({pos.X:F1},{pos.Y:F1})");
      _pointer.Last = pos;
    };

    PointerReleased += (_, e) => {
      Log.Debug("PointerReleased");
      SwipeHandler();
    };

    PointerCaptureLost += (_, e) => {
      Log.Debug("PointerCaptureLost");
      SwipeHandler();
    };
  }

  private void SwipeHandler() {
    var start = _pointer.Start;
    var last = _pointer.Last;
    var deltaX = last.X - start.X;
    var deltaY = last.Y - start.Y;
    Log.Debug($"SwipeHandler {deltaX:F1},{deltaY:F1}");

    // Confirm horizontal swipe and threshold
    if (Math.Abs(deltaX) >= SwipeThreshold && Math.Abs(deltaX) > Math.Abs(deltaY)) {
      if (deltaX < 0) {
        Log.Debug($"Swipe recognized deltaX={deltaX:F1} -> Next");
        Next();
      }
      else {
        Log.Debug($"Swipe recognized deltaX={deltaX:F1} -> Previous");
        Previous();
      }
    }

    // Confirm pull down and threshold
    else if (Math.Abs(deltaY) >= SwipeThreshold && Math.Abs(deltaY) > Math.Abs(deltaX) && deltaY > 0) {
      Log.Debug($"Pull-down recognized deltaY={deltaY:F1} -> OnPullDown");
      OnPullDown.Invoke(this, EventArgs.Empty);
    }
  }
  #endregion
}