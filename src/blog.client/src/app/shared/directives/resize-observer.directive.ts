import { Directive, ElementRef, EventEmitter, NgZone, OnDestroy, Output } from '@angular/core';

@Directive({
  selector: '[appResizeObserver]',
  standalone: true
})
export class ResizeObserverDirective implements OnDestroy {
  @Output() resize = new EventEmitter<DOMRectReadOnly>();

  private resizeObserver: ResizeObserver;

  constructor(private element: ElementRef, private ngZone: NgZone) {
    this.resizeObserver = new ResizeObserver(entries => {
      for (let entry of entries) {
        this.ngZone.run(() => this.resize.emit(entry.contentRect));
      }
    });

    this.resizeObserver.observe(this.element.nativeElement);
  }

  ngOnDestroy() {
    this.resizeObserver.disconnect();
  }
}
