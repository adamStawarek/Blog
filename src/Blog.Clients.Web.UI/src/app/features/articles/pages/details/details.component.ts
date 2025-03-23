import { CommonModule, DatePipe } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { MatDivider } from '@angular/material/divider';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { Client, GetArticleResponse } from 'src/app/core/api.generated';

@Component({
  selector: 'app-article-details',
  standalone: true,
  imports: [
    CommonModule,
    MatDivider,
    MatButtonModule,
    MatChipsModule,
    RouterModule,
    DatePipe],
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
