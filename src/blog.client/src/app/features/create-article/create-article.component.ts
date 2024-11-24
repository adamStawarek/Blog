import { CommonModule } from '@angular/common';
import { Component, OnDestroy, signal, WritableSignal } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatChipInputEvent, MatChipsModule } from '@angular/material/chips';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { Router } from '@angular/router';
import { AngularEditorConfig, AngularEditorModule } from '@kolkov/angular-editor';
import { Subject, takeUntil } from 'rxjs';
import { Client, CreateArticleRequest } from 'src/app/core/services/api.generated';

@Component({
  selector: 'app-create-article',
  standalone: true,
  imports: [
    AngularEditorModule,
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatIconModule,
    MatChipsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule],
  templateUrl: './create-article.component.html',
  styleUrl: './create-article.component.scss'
})
export class CreateArticleComponent implements OnDestroy {
  public editForm!: FormGroup;
  public tags: WritableSignal<string[]> = signal([]);
  public config: AngularEditorConfig = {
    editable: true,
    spellcheck: true,
    height: 'calc(100vh - 425px)',
    translate: 'no',
    defaultFontSize: '4',
    defaultParagraphSeparator: 'p',
    defaultFontName: 'Arial',
    toolbarHiddenButtons: [
      ['bold']
    ],
    customClasses: [
      {
        name: "quote",
        class: "quote",
      },
      {
        name: 'redText',
        class: 'redText'
      },
      {
        name: "titleText",
        class: "titleText",
        tag: "h1",
      },
    ]
  };

  private _destroy$: Subject<void> = new Subject<void>()

  constructor(fb: FormBuilder,
    private readonly _router: Router,
    private readonly _apiClient: Client) {

    this.editForm = fb.group({
      title: ['', [
        Validators.required,
        Validators.minLength(3),
        Validators.maxLength(200)]],
      description: ['', [
        Validators.required,
        Validators.minLength(20),
        Validators.maxLength(400)]],
      tags: ['', [
        Validators.required,
      ]],
      content: ['', [
        Validators.required
      ]]
    });
  }

  ngOnDestroy(): void {
    this._destroy$.next();
    this._destroy$.complete();
  }

  public removeTag(tag: string): void {
    this.tags.update(keywords => {
      const index = keywords.indexOf(tag);
      if (index < 0) {
        return keywords;
      }

      keywords.splice(index, 1);
      return [...keywords];
    });
  }

  public addTag(event: MatChipInputEvent): void {
    const value = (event.value || '').trim();

    if (value) {
      this.tags.update(keywords => [...keywords, value]);
    }

    event.chipInput!.clear();
  }

  public onCancel(): void {
    this._router.navigate(['']);
  }

  public onSubmit(): void {
    if (this.editForm.invalid) return;

    const request: CreateArticleRequest = {
      title: this.editForm.get('title')?.value,
      description: this.editForm.get('description')?.value,
      content: this.editForm.get('content')?.value,
      tags: this.editForm.get('tags')?.value,
    }
    this._apiClient.createArticle(request)
      .pipe(takeUntil(this._destroy$))
      .subscribe(() => {
        this._router.navigate(['']);
      });
  }
}
