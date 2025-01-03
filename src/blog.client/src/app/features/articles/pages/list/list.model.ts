import { CollectionViewer, DataSource } from "@angular/cdk/collections";
import { BehaviorSubject, Observable, Subscription, take } from "rxjs";
import { ArticleItem, Client } from "src/app/core/api.generated";

export class ArticlesDataSource extends DataSource<ArticleItem | undefined> {
  private _length = 100;
  private _pageSize = 15;
  private _cachedData = Array.from<ArticleItem>({ length: this._length });
  private _fetchedPages = new Set<number>();

  private readonly _dataStream = new BehaviorSubject<(ArticleItem | undefined)[]>(this._cachedData);
  private readonly _subscription = new Subscription();

  constructor(private _apiClient: Client) {
    super();
  }

  public connect(collectionViewer: CollectionViewer): Observable<(ArticleItem | undefined)[]> {
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

  public disconnect(): void {
    this._subscription.unsubscribe();
  }

  public refresh(): void {
    this._fetchedPages = new Set<number>();
    this._fetchPage(0);
  }

  private _getPageForIndex(index: number): number {
    return Math.floor(index / this._pageSize);
  }

  private _fetchPage(page: number): void {
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
