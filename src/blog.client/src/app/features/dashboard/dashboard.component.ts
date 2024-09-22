import { ScrollingModule } from '@angular/cdk/scrolling';
import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDividerModule } from '@angular/material/divider';
import { Router } from '@angular/router';
import { ArticleItem, Client } from 'src/app/core/services/api.generated';
import { ArticlesDataSource } from './dashboard.model';

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

  constructor(apiClient: Client, private readonly _router: Router) {
    this.articleDataSource = new ArticlesDataSource(apiClient);
  }

  public navigateToArticle(article: ArticleItem): void {
    this._router.navigate(['articles', article.id]);
  }
}

