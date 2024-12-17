import { ScrollingModule } from '@angular/cdk/scrolling';
import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDividerModule } from '@angular/material/divider';
import { Router } from '@angular/router';
import { ArticleItem, Client } from 'src/app/core/services/api.generated';
import { ArticlesDataSource } from './list.model';

@Component({
  selector: 'app-list',
  standalone: true,
  imports: [
    CommonModule,
    MatDividerModule,
    ScrollingModule,
    MatCardModule,
    MatButtonModule],
  templateUrl: './list.component.html',
  styleUrl: './list.component.scss'
})
export class ArticleListComponent {

  public articleDataSource: ArticlesDataSource;

  constructor(apiClient: Client, private readonly _router: Router) {
    this.articleDataSource = new ArticlesDataSource(apiClient);
  }

  public navigateToArticle(article: ArticleItem): void {
    this._router.navigate(['articles', article.id]);
  }
}
