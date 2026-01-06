import { COMMA, ENTER } from '@angular/cdk/keycodes';
import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnChanges, OnDestroy, Output, signal, SimpleChanges, WritableSignal } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatChipInputEvent, MatChipsModule } from '@angular/material/chips';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { AngularEditorConfig, AngularEditorModule } from '@kolkov/angular-editor';
import { Subject } from 'rxjs';
import { ArticleData } from './article-editor.model';

@Component({
  selector: 'app-article-editor',
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
    MatCheckboxModule,
    MatButtonModule],
  templateUrl: './article-editor.component.html',
  styleUrl: './article-editor.component.scss'
})
export class ArticleEditorComponent implements OnDestroy, OnChanges {
  @Input() public article: ArticleData | null = null;

  @Output() public cancel = new EventEmitter<void>();
  @Output() public save = new EventEmitter<ArticleData>();

  public editForm!: FormGroup;
  public tags: WritableSignal<string[]> = signal([]);
  public config: AngularEditorConfig = {
    editable: true,
    spellcheck: true,
    height: 'calc(100vh - 400px)',
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

  public readonly TagSeparatorKeys = [ENTER, COMMA] as const;

  private _destroy$: Subject<void> = new Subject<void>();

  constructor(fb: FormBuilder) {
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
      isDraft: ['', [
        Validators.required,
      ]],
      content: ['', [
        Validators.required
      ]]
    });
  }

  ngOnChanges(_changes: SimpleChanges): void {
    if (!_changes['article']) return;

    this.editForm.controls['title'].setValue(this.article!.title);
    this.editForm.controls['description'].setValue(this.article!.description);
    this.editForm.controls['tags'].setValue(this.article!.tags);
    this.editForm.controls['isDraft'].setValue(this.article!.isDraft);
    this.editForm.controls['content'].setValue(this.article!.content);

    this.tags.update(() => [...this.article!.tags]);
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
    this.cancel.emit();
  }

  public onSubmit(): void {
    if (this.editForm.invalid) return;

    this.editForm.markAsPristine();
    this.save.emit({
      title: this.editForm.get('title')?.value,
      description: this.editForm.get('description')?.value,
      content: this.editForm.get('content')?.value,
      tags: this.editForm.get('tags')?.value,
      isDraft: this.editForm.get('isDraft')?.value,
    });
  }
}
