import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { Client } from 'src/app/core/services/api.generated';

@Component({
  selector: 'app-article',
  standalone: true,
  imports: [],
  templateUrl: './article.component.html',
  styleUrl: './article.component.scss'
})
export class ArticleComponent implements OnInit, OnDestroy {
  private _destroy$: Subject<void> = new Subject<void>();

  constructor(private readonly apiClient: Client, private readonly activatedRoute: ActivatedRoute) { }

  ngOnInit(): void {
    this.fetchArticle();
  }

  ngOnDestroy(): void {
    this._destroy$.next();
    this._destroy$.complete();
  }

  private fetchArticle(): void {
    const articleId = this.activatedRoute.snapshot.paramMap.get('id')!;
    this.apiClient.getArticle(articleId)
      .pipe(takeUntil(this._destroy$))
      .subscribe((article) => {

      });
  }
}
