import { CommonModule, DatePipe } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatBadgeModule } from '@angular/material/badge';
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { MatDividerModule } from '@angular/material/divider';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { Observable, Subject, takeUntil } from 'rxjs';
import { Client, Comment, CreateCommentRequest, GetArticleResponse } from 'src/app/core/api.generated';
import { AuthService } from 'src/app/core/auth.service';

@Component({
  selector: 'app-article-details',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatDividerModule,
    MatSnackBarModule,
    MatButtonModule,
    MatChipsModule,
    MatBadgeModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    RouterModule,
    DatePipe],
  templateUrl: './details.component.html',
  styleUrl: './details.component.scss'
})
export class ArticleDetailsComponent implements OnInit, OnDestroy {
  public article?: GetArticleResponse;
  public selectedComment?: Comment;

  public isLoggedIn$: Observable<boolean> = this._authSerive.isAuthenticated$;

  private _destroy$: Subject<void> = new Subject<void>();

  constructor(
    private readonly _snakBar: MatSnackBar,
    private readonly _apiClient: Client,
    private readonly _activatedRoute: ActivatedRoute,
    private readonly _authSerive: AuthService) { }

  ngOnInit(): void {
    this.fetchArticle();
  }

  ngOnDestroy(): void {
    this._destroy$.next();
    this._destroy$.complete();
  }

  public publishComment(comment: string, parentId?: string): void {
    const request: CreateCommentRequest = {
      content: comment,
      parentId: parentId,
    }
    this._apiClient.createComment(this.article!.id, request)
      .pipe(takeUntil(this._destroy$))
      .subscribe((id: string) => {
        const newComment: Comment = {
          id: id,
          content: comment,
          date: new Date(),
          author: 'Me',
          replies: [],
        };

        if (parentId) {
          const parentComment = this.article!.comments.find(c => c.id === parentId);
          parentComment!.replies.push(newComment);
        } else {
          this.article!.comments.unshift(newComment);
        }

        this._snakBar.open('Successfully published new comment!', 'Close', {
          duration: 3000,
          panelClass: ['notification-success']
        })
      });
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
