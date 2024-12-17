import { Component, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { Client, CreateArticleRequest } from 'src/app/core/services/api.generated';
import { ArticleEditorComponent } from "../../components/article-editor/article-editor.component";
import { ArticleData } from '../../components/article-editor/article-editor.model';

@Component({
  selector: 'app-create',
  standalone: true,
  imports: [ArticleEditorComponent],
  templateUrl: './create.component.html',
  styleUrl: './create.component.scss'
})
export class CreateArticleComponent implements OnDestroy {

  private _destroy$: Subject<void> = new Subject<void>();

  constructor(
    private readonly _router: Router,
    private readonly _apiClient: Client) { }

  ngOnDestroy(): void {
    this._destroy$.next();
    this._destroy$.complete();
  }

  public onCancel(): void {
    this._router.navigate(['']);
  }

  public onSubmit($event: ArticleData): void {
    const request: CreateArticleRequest = {
      title: $event.title,
      description: $event.description,
      content: $event.content,
      tags: $event.tags
    }

    this._apiClient.createArticle(request)
      .pipe(takeUntil(this._destroy$))
      .subscribe(() => {
        this._router.navigate(['']);
      });
  }
}
