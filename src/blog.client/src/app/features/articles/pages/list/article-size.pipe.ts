import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'articleSize',
  standalone: true
})
export class ArticleSizePipe implements PipeTransform {
  public transform(rect: DOMRectReadOnly | undefined): number {
    const itemHeight = 110.5;
    if (!rect) return itemHeight;

    const minItemWidth = 280;
    const divider = Math.floor(rect.width / minItemWidth);
    const result = itemHeight / divider;
    return result < 1 ? 1 : Math.floor(result);
  }
}
