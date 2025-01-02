import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from 'src/app/core/auth.guard';

const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./pages/list/list.component').then(m => m.ArticleListComponent)
  },
  {
    path: 'create',
    data: {
      rawPage: true,
      // roles: ['Admin']
    },
    canActivate: [AuthGuard],
    loadComponent: () => import('./pages/create/create.component').then(m => m.CreateArticleComponent),
  },
  {
    path: ':id/edit',
    data: {
      rawPage: true,
      roles: ['Admin']
    },
    canActivate: [AuthGuard],
    loadComponent: () => import('./pages/edit/edit.component').then(m => m.EditArticleComponent),
  },
  {
    path: ':id',
    loadComponent: () => import('./pages/details/details.component').then(m => m.ArticleDetailsComponent),
  }
];
@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    RouterModule.forChild(routes)
  ]
})
export class ArticlesModule { }
