import os
import glob

def convert_csv_to_utf8():
    # 👉 [추가된 부분] 스크립트 자신의 실제 위치를 알아내서, 작업 기준 폴더를 그곳으로 강제 변경합니다.
    # 1. 현재 스크립트가 실행되는 폴더 내의 모든 .csv 파일 검색
    script_dir = os.path.dirname(os.path.abspath(__file__))
    os.chdir(script_dir)

    # 1. 현재 스크립트가 실행되는 폴더 내의 모든 .csv 파일 검색
    csv_files = glob.glob('*.[cC][sS][vV]')
    
    if not csv_files:
        print("현재 폴더에 변환할 CSV 파일이 없습니다.")
        return

    print(f"총 {len(csv_files)}개의 CSV 파일을 발견했습니다. 변환을 시작합니다...\n")
    
    success_count = 0
    error_count = 0

    for file in csv_files:
        try:
            # 2. 기존 파일을 한국어 윈도우 기본 인코딩(CP949/ANSI)으로 읽기
            with open(file, 'r', encoding='cp949') as f:
                content = f.read()

            # 3. 읽어온 내용을 동일한 파일명에 UTF-8 인코딩으로 덮어쓰기
            # 유니티에서 BOM(Byte Order Mark) 문제가 생기지 않도록 순수 'utf-8'을 사용합니다.
            with open(file, 'w', encoding='utf-8') as f:
                f.write(content)
            
            print(f"✅ [성공] {file}")
            success_count += 1

        except UnicodeDecodeError:
            print(f"⚠️ [건너뜀] {file} - 이미 UTF-8로 되어있거나 다른 인코딩 방식입니다.")
            error_count += 1
        except Exception as e:
            print(f"❌ [에러] {file} - 문제 발생: {e}")
            error_count += 1

    # 4. 결과 요약
    print("-" * 40)
    print(f"🎉 작업 완료! (성공: {success_count}개 / 건너뜀 및 에러: {error_count}개)")

if __name__ == "__main__":
    convert_csv_to_utf8()
    input("\n엔터(Enter) 키를 누르면 창이 닫힙니다...")