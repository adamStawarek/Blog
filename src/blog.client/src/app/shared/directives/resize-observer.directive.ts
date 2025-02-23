import { Directive, ElementRef, EventEmitter, NgZone, OnDestroy, Output } from '@angular/core';

@Directive({
  selector: '[appResizeObserver]',
  standalone: true
})
export class ResizeObserverDirective implements OnDestroy {
  @Output() public sizeChanged = new EventEmitter<DOMRectReadOnly>();

  private _resizeObserver: ResizeObserver;

  constructor(private element: ElementRef, private ngZone: NgZone) {
    this._resizeObserver = new ResizeObserver(entries => {
      for (const entry of entries) {
        this.ngZone.run(() => this.sizeChanged.emit(entry.contentRect));
      }
    });

    this._resizeObserver.observe(this.element.nativeElement);
  }

  ngOnDestroy(): void {
    this._resizeObserver.disconnect();
  }
}
