import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
    name: 'articleSize',
    standalone: true,
})
export class ArticleSizePipe implements PipeTransform {
    public transform(containerWidth: number): number {
        const itemHeight = 130;
        if (!containerWidth) return itemHeight;

        const minItemWidth = 280;
        const divider = Math.floor(containerWidth / minItemWidth);
        const result = itemHeight / divider;
        return result < 1 ? 1 : Math.floor(result);
    }
}