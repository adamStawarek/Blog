import { ScrollingModule } from '@angular/cdk/scrolling';
import { CommonModule } from '@angular/common';
import { Component, OnDestroy } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDividerModule } from '@angular/material/divider';
import { MatIconModule } from '@angular/material/icon';
import { Router, RouterModule } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { ArticleItem, Client } from 'src/app/core/api.generated';
import { AuthService } from 'src/app/core/auth.service';
import { ArticlesDataSource } from './list.model';

@Component({
  selector: 'app-article-list',
  standalone: true,
  imports: [
    CommonModule,
    MatDividerModule,
    ScrollingModule,
    RouterModule,
    MatCardModule,
    MatIconModule,
    MatButtonModule],
  templateUrl: './list.component.html',
  styleUrl: './list.component.scss'
})
export class ArticleListComponent implements OnDestroy {
  public articleDataSource: ArticlesDataSource;

  private _destroy$: Subject<void> = new Subject<void>();

  constructor(
    private readonly _apiClient: Client,
    private readonly _router: Router,
    public readonly authService: AuthService) {
    this.articleDataSource = new ArticlesDataSource(_apiClient);
  }

  ngOnDestroy(): void {
    this._destroy$.next();
    this._destroy$.complete();
  }

  public navigateToArticle(article: ArticleItem): void {
    this._router.navigate(['articles', article.id]);
  }

  public createArticle($event: Event): void {
    $event.stopPropagation();

    this._router.navigate(['articles', 'create']);
  }

  public editArticle($event: Event, article: ArticleItem): void {
    $event.stopPropagation();

    this._router.navigate(['articles', article.id, 'edit']);
  }

  public deleteArticle($event: Event, article: ArticleItem): void {
    $event.stopPropagation();

    if (!confirm(`Are you sure to permanently delete '${article.title}' article?`)) {
      return;
    }

    this._apiClient.deleteArticle(article.id)
      .pipe(takeUntil(this._destroy$))
      .subscribe(() => {
        this.articleDataSource.refresh();
      });
  }
}
