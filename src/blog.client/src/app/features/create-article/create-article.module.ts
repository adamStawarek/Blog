import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { Route, RouterModule } from '@angular/router';
import { CreateArticleComponent } from './create-article.component';

export const routes: Route[] = [
  {
    path: '',
    component: CreateArticleComponent
  }
];

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    RouterModule.forChild(routes),
  ]
})
export class CreateArticleModule { }
