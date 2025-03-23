import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatIconRegistry } from '@angular/material/icon';
import { DomSanitizer } from '@angular/platform-browser';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { filter, map, Subscription } from 'rxjs';
import jsonData from 'src/assets/data.json';
import { BlogData } from './app.model';
import { AuthService } from './core/auth.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit, OnDestroy {
  public rawPage = false;
  public compactView = false;
  public blogData: BlogData = jsonData;

  private _destroy$?: Subscription;

  constructor(
    private readonly router: Router,
    private readonly activatedRoute: ActivatedRoute,
    private readonly matIconRegistry: MatIconRegistry,
    private readonly domSanitizer: DomSanitizer,
    private readonly authService: AuthService
  ) {
    this.matIconRegistry.addSvgIcon('github', this.domSanitizer.bypassSecurityTrustResourceUrl('../assets/github.svg'));
    this.matIconRegistry.addSvgIcon('linkedin', this.domSanitizer.bypassSecurityTrustResourceUrl('../assets/linkedin.svg'));
  }

  ngOnInit(): void {
    this.authService.init();

    this.compactView = window.innerWidth <= 768;

    this._destroy$ = this.router.events
      .pipe(
        filter((event) => event instanceof NavigationEnd), // Only process NavigationEnd events
        map(() => this.getChildRoute(this.activatedRoute)), // Get the deepest activated route
        map((route) => route.snapshot) // Get the route snapshot
      )
      .subscribe((snapshot) => {
        this.rawPage = snapshot.data['rawPage'] ?? false;
      });
  }

  ngOnDestroy(): void {
    this._destroy$?.unsubscribe();
  }

  // Helper function to get the deepest active route
  private getChildRoute(route: ActivatedRoute): ActivatedRoute {
    while (route.firstChild) {
      route = route.firstChild;
    }
    return route;
  }
}
