import { Component, inject, signal } from '@angular/core';
import { Region } from './models/region';
import { RegionService } from './services/region';

@Component({
  selector: 'app-root',
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App {
  private readonly regionService = inject(RegionService);

  protected readonly regions = signal<Region[]>([]);
  protected readonly loadFailed = signal(false);

  constructor() {
    this.regionService.getAll().subscribe({
      next: (regions) => this.regions.set(regions),
      error: () => this.loadFailed.set(true),
    });
  }
}
