
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  {
    path: '',
    loadChildren: () => import('./features/dashboard/dashboard.module').then(m => m.DashboardModule),
  },
  {
    path: 'articles',
    loadChildren: () => import('./features/article/article.module').then(m => m.ArticleModule),
  },
  {
    path: 'create-article',
    data: {
      rawPage: true
    },
    loadChildren: () => import('./features/create-article/create-article.module').then(m => m.CreateArticleModule),
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
