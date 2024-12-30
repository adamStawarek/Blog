import { DatePipe } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatDivider } from '@angular/material/divider';
import { ActivatedRoute } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { Client, GetArticleResponse } from 'src/app/core/api.generated';

@Component({
  selector: 'app-article-details',
  standalone: true,
  imports: [MatDivider, DatePipe],
  templateUrl: './details.component.html',
  styleUrl: './details.component.scss'
})
export class ArticleDetailsComponent implements OnInit, OnDestroy {
  public article?: GetArticleResponse;

  private _destroy$: Subject<void> = new Subject<void>();

  constructor(
    private readonly _apiClient: Client,
    private readonly _activatedRoute: ActivatedRoute) { }

  ngOnInit(): void {
    this.fetchArticle();
  }

  ngOnDestroy(): void {
    this._destroy$.next();
    this._destroy$.complete();
  }

  private fetchArticle(): void {
    const articleId = this._activatedRoute.snapshot.paramMap.get('id')!;
    this._apiClient.getArticle(articleId)
      .pipe(takeUntil(this._destroy$))
      .subscribe((article) => {
        this.article = article
      });
  }
}
