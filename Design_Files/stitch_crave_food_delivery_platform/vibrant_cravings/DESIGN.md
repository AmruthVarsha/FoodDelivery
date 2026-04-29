---
name: Vibrant Cravings
colors:
  surface: '#f4fafd'
  surface-dim: '#d4dbdd'
  surface-bright: '#f4fafd'
  surface-container-lowest: '#ffffff'
  surface-container-low: '#eef5f7'
  surface-container: '#e8eff1'
  surface-container-high: '#e2e9ec'
  surface-container-highest: '#dde4e6'
  on-surface: '#161d1f'
  on-surface-variant: '#5c403a'
  inverse-surface: '#2b3234'
  inverse-on-surface: '#ebf2f4'
  outline: '#916f69'
  outline-variant: '#e5bdb6'
  surface-tint: '#ba1c00'
  primary: '#b51c00'
  on-primary: '#ffffff'
  primary-container: '#dc3214'
  on-primary-container: '#fffbff'
  inverse-primary: '#ffb4a5'
  secondary: '#006d37'
  on-secondary: '#ffffff'
  secondary-container: '#6bfe9c'
  on-secondary-container: '#00743a'
  tertiary: '#725c00'
  on-tertiary: '#ffffff'
  tertiary-container: '#cca700'
  on-tertiary-container: '#4d3e00'
  error: '#ba1a1a'
  on-error: '#ffffff'
  error-container: '#ffdad6'
  on-error-container: '#93000a'
  primary-fixed: '#ffdad3'
  primary-fixed-dim: '#ffb4a5'
  on-primary-fixed: '#3e0400'
  on-primary-fixed-variant: '#8e1300'
  secondary-fixed: '#6bfe9c'
  secondary-fixed-dim: '#4ae183'
  on-secondary-fixed: '#00210c'
  on-secondary-fixed-variant: '#005228'
  tertiary-fixed: '#ffe07e'
  tertiary-fixed-dim: '#ebc31d'
  on-tertiary-fixed: '#231b00'
  on-tertiary-fixed-variant: '#564500'
  background: '#f4fafd'
  on-background: '#161d1f'
  surface-variant: '#dde4e6'
typography:
  display-lg:
    fontFamily: Plus Jakarta Sans
    fontSize: 48px
    fontWeight: '800'
    lineHeight: 56px
    letterSpacing: -0.02em
  headline-lg:
    fontFamily: Plus Jakarta Sans
    fontSize: 32px
    fontWeight: '700'
    lineHeight: 40px
    letterSpacing: -0.01em
  headline-md:
    fontFamily: Plus Jakarta Sans
    fontSize: 24px
    fontWeight: '700'
    lineHeight: 32px
  body-lg:
    fontFamily: Work Sans
    fontSize: 18px
    fontWeight: '400'
    lineHeight: 28px
  body-md:
    fontFamily: Work Sans
    fontSize: 16px
    fontWeight: '400'
    lineHeight: 24px
  label-lg:
    fontFamily: Work Sans
    fontSize: 14px
    fontWeight: '600'
    lineHeight: 20px
    letterSpacing: 0.01em
  label-sm:
    fontFamily: Work Sans
    fontSize: 12px
    fontWeight: '500'
    lineHeight: 16px
rounded:
  sm: 0.25rem
  DEFAULT: 0.5rem
  md: 0.75rem
  lg: 1rem
  xl: 1.5rem
  full: 9999px
spacing:
  unit: 4px
  xs: 4px
  sm: 8px
  md: 16px
  lg: 24px
  xl: 40px
  gutter: 16px
  margin-mobile: 20px
  margin-desktop: 64px
---

## Brand & Style

The design system is built on a foundation of "Energetic Modernism." It aims to trigger immediate sensory appeal while maintaining a high level of functional reliability. The brand personality is friendly and approachable, yet underscores the speed and efficiency essential for a delivery service.

To achieve this, the system utilizes a **Modern-Tactile** style. It leverages high-resolution, full-bleed imagery of food to drive desire, supported by a UI that feels soft and physical. Surfaces are clean and breathable, using subtle depth to guide the user’s eye toward "crave-worthy" choices and streamlined conversion paths.

## Colors

The palette is anchored by **Deep Orange (#FF4B2B)**, a high-chroma primary color selected for its physiological association with appetite and urgency. This is balanced by **Fresh Green (#2ECC71)**, used specifically for "Healthy Choice" badges, vegan options, and sustainability indicators to evoke freshness and vitality.

**Neutral tones** are kept light and cool to ensure the photography remains the focal point. Surfaces use a clean white base with very light gray elevations to define hierarchy without adding visual weight. Success states use the secondary green, while warnings utilize the primary orange or a secondary yellow to maintain brand harmony.

## Typography

The typography strategy employs a pairing that balances personality with extreme utility. **Plus Jakarta Sans** is used for headlines; its soft, rounded terminals provide the "friendly" brand character while its bold weights create a clear visual anchor for dish names and restaurant titles.

**Work Sans** is used for all body copy and UI labels. It was chosen for its exceptional legibility at small sizes, particularly crucial for ingredient lists, nutritional facts, and delivery ETAs. To maintain a modern feel, tight tracking is used on large headlines, while standard tracking is applied to body text to ensure maximum readability during the ordering process.

## Layout & Spacing

This design system utilizes a **Fluid Grid** model with a base-4 spatial rhythm. On mobile devices, a 4-column grid is used with 20px side margins to allow food photography to feel expansive. For desktop, a 12-column grid ensures content is organized into logical groups like "Filters," "Restaurant List," and "Cart."

Spacing is intentionally generous around high-quality imagery to prevent a cluttered "discount" feel. Vertical rhythm is driven by the internal content weight, with larger gaps (40px+) between major sections (e.g., "Recently Ordered" vs "Local Favorites") to provide clear mental breaks for the user.

## Elevation & Depth

The design system uses **Ambient Shadows** to create a sense of layering and "touchability." Shadows are not neutral gray; they are subtly tinted with the primary orange or a deep navy to keep the interface feeling warm and vibrant.

1.  **Level 0 (Base):** Flat background for the main interface.
2.  **Level 1 (Cards):** Low-offset, highly diffused shadow (Blur: 12px, Opacity: 6%) used for restaurant and menu item cards to make them appear slightly lifted.
3.  **Level 2 (Navigation/Floating):** Higher offset shadow (Blur: 20px, Opacity: 10%) used for the bottom navigation bar and floating "View Cart" buttons to signify they sit above all other content.
4.  **Interactive:** On hover or press, cards should slightly increase their shadow spread and scale by 1-2% to provide tactile feedback.

## Shapes

The shape language is defined by **Rounded (Level 2)** corners, reinforcing the friendly and approachable brand personality. 

- **Cards and Containers:** Use a 1rem (16px) corner radius to soften the edges of the UI.
- **Buttons:** Use a fully pill-shaped (rounded-full) radius to make them feel inviting to tap.
- **Media:** Food photography should always follow the container’s corner radius to ensure a cohesive, integrated look. 
- **Icons:** Use a 2px stroke weight with rounded caps and joins to match the typography's softness.

## Components

**Buttons**
Primary buttons are pill-shaped, filled with Deep Orange, and use white bold text. They feature a subtle inner glow to enhance their "appetizing" look. Secondary buttons use a light orange tint with orange text.

**Cards**
Restaurant cards are the hero of the system. They feature full-bleed imagery at the top with a 16px bottom padding for content. Content within the card (title, rating, ETA) is aligned to a strict grid to maintain order amidst vibrant photos.

**Chips & Badges**
Used for dietary filters (e.g., "Vegan," "Gluten-Free"). These use the Secondary Green for health-related tags and a light neutral for others. They are small, pill-shaped, and use `label-sm` typography.

**Inputs**
Search bars and form fields use a light gray surface (#F8F9FA) with a 16px radius. On focus, the border transitions to a 2px Deep Orange stroke.

**Crave-Specific Components**
- **Live Tracker:** A progress-based component using a vibrant gradient from Deep Orange to Fresh Green, signifying the journey from kitchen to doorstep.
- **Food Story:** A circular, bordered component for "Featured Dishes" at the top of the home screen, mimicking social media story patterns.