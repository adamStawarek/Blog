import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { LoaderService } from 'src/app/core/loader.service';

@Component({
  selector: 'app-spinner',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './spinner.component.html',
  styleUrl: './spinner.component.scss'
})
export class SpinnerComponent {
  @Input() public size = 2;

  constructor(public loader: LoaderService) { }
}
