
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  { path: '', redirectTo: '/articles', pathMatch: 'full' },
  {
    path: 'articles',
    loadChildren: () => import('./features/articles/articles.module').then(m => m.ArticlesModule)
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
