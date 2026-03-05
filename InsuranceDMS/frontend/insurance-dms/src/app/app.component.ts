import { Component } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  template: `
    <div class="layout">
      <aside class="sidebar">
        <div class="sidebar-logo">Insurance DMS</div>
        <nav class="sidebar-nav">
          <a class="nav-item" routerLink="/dashboard" routerLinkActive="active">
            <span class="nav-icon">◉</span> Dashboard
          </a>
          <a class="nav-item" routerLink="/agencies" routerLinkActive="active">
            <span class="nav-icon">🏢</span> Agencies
          </a>
          <a class="nav-item" routerLink="/personnel" routerLinkActive="active">
            <span class="nav-icon">👤</span> Personnel
          </a>
          <a class="nav-item" routerLink="/licensing" routerLinkActive="active">
            <span class="nav-icon">📋</span> Licensing
          </a>
          <a class="nav-item" routerLink="/appointments" routerLinkActive="active">
            <span class="nav-icon">📌</span> Appointments
          </a>
          <a class="nav-item" routerLink="/mergers" routerLinkActive="active">
            <span class="nav-icon">🔀</span> Mergers & Acq.
          </a>
        </nav>
      </aside>
      <main class="main-content">
        <router-outlet />
      </main>
    </div>
  `
})
export class AppComponent {}
