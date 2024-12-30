import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { Client, EditArticleRequest } from 'src/app/core/api.generated';
import { ArticleEditorComponent } from "../../components/article-editor/article-editor.component";
import { ArticleData } from '../../components/article-editor/article-editor.model';

@Component({
  selector: 'app-article-edit',
  standalone: true,
  imports: [CommonModule, ArticleEditorComponent],
  templateUrl: './edit.component.html',
  styleUrl: './edit.component.scss'
})
export class EditArticleComponent implements OnInit, OnDestroy {
  public articleId!: string;
  public article!: ArticleData;

  private _destroy$: Subject<void> = new Subject<void>();

  constructor(
    private readonly _router: Router,
    private readonly _activatedRoute: ActivatedRoute,
    private readonly _apiClient: Client) {

    this.articleId = this._activatedRoute.snapshot.paramMap.get('id')!;
  }

  ngOnInit(): void {
    this.fetchArticle();
  }

  ngOnDestroy(): void {
    this._destroy$.next();
    this._destroy$.complete();
  }

  public onCancel(): void {
    this._router.navigate(['']);
  }

  public onSave($event: ArticleData): void {
    const request: EditArticleRequest = {
      title: $event.title,
      description: $event.description,
      content: $event.content,
      tags: $event.tags
    }

    this._apiClient.editArticle(this.articleId, request)
      .pipe(takeUntil(this._destroy$))
      .subscribe(() => {
        this._router.navigate(['']);
      });
  }

  private fetchArticle(): void {
    this._apiClient.getArticle(this.articleId)
      .pipe(takeUntil(this._destroy$))
      .subscribe((article) => {
        this.article = {
          title: article.title,
          description: article.description,
          content: article.content,
          tags: article.tags
        }
      });
  }
}
