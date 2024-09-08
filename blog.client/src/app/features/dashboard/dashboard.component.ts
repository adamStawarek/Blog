import { CollectionViewer, DataSource } from '@angular/cdk/collections';
import { ScrollingModule } from '@angular/cdk/scrolling';
import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDividerModule } from '@angular/material/divider';
import { BehaviorSubject, Observable, Subscription, take } from 'rxjs';
import { ArticleItem, Client } from 'src/app/core/services/api.generated';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    MatDividerModule,
    ScrollingModule,
    MatCardModule,
    MatButtonModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent {
  public articleDataSource: ArticlesDataSource;

  constructor(apiClient: Client) {
    this.articleDataSource = new ArticlesDataSource(apiClient);
  }
}

export class ArticlesDataSource extends DataSource<ArticleItem | undefined> {
  constructor(private _apiClient: Client) {
    super();
  }

  private _length = 1000;
  private _pageSize = 10;
  private _cachedData = Array.from<ArticleItem>({ length: this._length });
  private _fetchedPages = new Set<number>();

  private readonly _dataStream = new BehaviorSubject<(ArticleItem | undefined)[]>(this._cachedData);
  private readonly _subscription = new Subscription();

  connect(collectionViewer: CollectionViewer): Observable<(ArticleItem | undefined)[]> {
    this._subscription.add(
      collectionViewer.viewChange.subscribe(range => {
        const startPage = this._getPageForIndex(range.start);
        const endPage = this._getPageForIndex(range.end - 1);
        for (let i = startPage; i <= endPage; i++) {
          this._fetchPage(i);
        }
      }),
    );
    return this._dataStream;
  }

  disconnect(): void {
    this._subscription.unsubscribe();
  }

  private _getPageForIndex(index: number): number {
    return Math.floor(index / this._pageSize);
  }

  private _fetchPage(page: number) {
    if (this._fetchedPages.has(page)) {
      return;
    }
    this._fetchedPages.add(page);

    this._apiClient.getArticles(page, this._pageSize)
      .pipe(take(1))
      .subscribe(x => {
        this._cachedData.length = x.totalItems;
        this._cachedData.splice(
          page * this._pageSize,
          this._pageSize,
          ...x.items,
        );
        this._dataStream.next(this._cachedData);

      });
  }
}
